using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.CommandLine;
using System.Text.Json;

namespace Invoice.SandboxRunner;

internal static class PathHelper
{
    // Türkçe: Çözüm kökünü bulmak için yukarı doğru yürür; Invoice.sln veya output klasörünü arar.
    public static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        // Türkçe: net9.0 -> Debug -> bin -> SandboxRunner -> tools -> Invoice (muhtemel kök)
        for (int i = 0; i < 10 && dir != null; i++, dir = dir.Parent)
        {
            if (dir.GetFiles("Invoice.sln", SearchOption.TopDirectoryOnly).Length > 0)
                return dir.FullName;

            if (dir.GetDirectories("output", SearchOption.TopDirectoryOnly).Length > 0 &&
                dir.GetDirectories("tools", SearchOption.TopDirectoryOnly).Length > 0 &&
                dir.GetDirectories("src", SearchOption.TopDirectoryOnly).Length > 0)
                return dir.FullName;
        }
        // Türkçe: Bulunamazsa çalışılan dizini döner.
        return AppContext.BaseDirectory;
    }

    // Türkçe: Güvenli klasör oluşturma yardımcıları
    public static string EnsureDir(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
}

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Türkçe: Varsayılan klasörler (çözüm köküne göre)
        var solutionRoot = PathHelper.FindSolutionRoot();
        var defaultLogsDir = Path.Combine(solutionRoot, "logs");
        var defaultRespDir = Path.Combine(solutionRoot, "output", "responses");

        // Türkçe: CLI parametreleri
        var kzAuth = new Option<bool>("--kz-auth", "KZ authentication test");
        var kzSend = new Option<FileInfo?>("--kz-send", "KZ invoice göndermek için XML dosya yolu");
        var uzAuth = new Option<bool>("--uz-auth", "UZ authentication test");
        var uzSend = new Option<FileInfo?>("--uz-send", "UZ invoice göndermek için JSON dosya yolu");
        var outputDirOpt = new Option<DirectoryInfo?>("--output-dir", () => null, "Response çıktı klasörü (opsiyonel)");
        var logDirOpt = new Option<DirectoryInfo?>("--log-dir", () => null, "Log klasörü (opsiyonel)");
        var envOpt = new Option<string?>("--env", () => null, "Konfigürasyon ortam adı (örn: Development, Production).");

        var root = new RootCommand("SandboxRunner");
        root.AddOption(kzAuth);
        root.AddOption(kzSend);
        root.AddOption(uzAuth);
        root.AddOption(uzSend);
        root.AddOption(outputDirOpt);
        root.AddOption(logDirOpt);
        root.AddOption(envOpt);

        // Türkçe: run-all alt komutunu ekliyoruz (tek atışta akış + validasyon + rapor)
        root.AddCommand(RunAllCommand.Build(outputDirOpt, logDirOpt, envOpt));

        // Türkçe: Gerçek mod öncesi kontrol komutu (env destekli)
        root.AddCommand(PreflightCommand.Build(envOpt));

        DirectoryInfo? outputDir = null;
        DirectoryInfo? logDir = null;

        root.SetHandler(async (bool doKzAuth, FileInfo? kzXml, bool doUzAuth, FileInfo? uzJson, DirectoryInfo? outDir, DirectoryInfo? logDirCli, string? envName) =>
        {
            // Türkçe: Çıktı ve log klasörlerini belirle
            outputDir = outDir ?? new DirectoryInfo(defaultRespDir);
            logDir = logDirCli ?? new DirectoryInfo(defaultLogsDir);
            PathHelper.EnsureDir(outputDir!.FullName);
            PathHelper.EnsureDir(logDir!.FullName);

            // Serilog: SIMULATION enrich'i
            var simulationSwitch = new LoggingLevelSwitch(LogEventLevel.Information);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(simulationSwitch)
                .Enrich.WithProperty("App", "SandboxRunner")
                .Enrich.WithProperty("Tag", SimulationMarkers.Tag) // [SIMULATION] iz bırakır
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logDir!.FullName, "sandbox-tests.log"),
                              rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                // Konfigürasyonu ConfigLoader ile yükle
                var configuration = ConfigLoader.Load(envName);

                // [SIMULATION] bayrağı
                var simulation = configuration.GetValue<bool>("Sandbox:Simulation");

                var services = new ServiceCollection();
                services.Configure<KzOptions>(configuration.GetSection(KzOptions.SectionName));
                services.Configure<UzOptions>(configuration.GetSection(UzOptions.SectionName));
                services.Configure<RetryPolicyOptions>(configuration.GetSection(RetryPolicyOptions.SectionName));

                services.AddLogging(b => b.AddSerilog());
                services.AddHttpClient<IKzEsfClient, KzEsfClient>();
                services.AddHttpClient<IUzDidoxClient, UzDidoxClient>();

                var sp = services.BuildServiceProvider();
                var logger = sp.GetRequiredService<ILogger<Program>>();

                // Yardımcı: response yaz (JSON'a "simulation" alanı eklenir)
                async Task WriteResponseAsync(string prefix, string content)
                {
                    // [SIMULATION] JSON işaretlemesi
                    var contentWithFlag = SimulationHelpers.EnsureSimulationFlag(content, simulation);

                    var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var file = Path.Combine(outputDir!.FullName, $"{prefix}_{ts}.json");
                    await File.WriteAllTextAsync(file, contentWithFlag);
                    logger.LogInformation("{Tag} Response kaydedildi: {Path}", SimulationMarkers.Tag, file);
                    Console.WriteLine($"[OK] {file}");
                }

                // [SIMULATION] Mock JSON üretici
                static string MockOk(string system, string action, bool simulation) => JsonSerializer.Serialize(new
                {
                    system,
                    action,
                    status = "OK",
                    token = "mock-token-123",
                    at = DateTime.UtcNow,
                    simulation // JSON içinde net görünür
                }, new JsonSerializerOptions { WriteIndented = true });

                // === Akışlar ===

                if (doKzAuth)
                {
                    if (simulation)
                    {
                        // [SIMULATION] KZ auth
                        await WriteResponseAsync("KZ_AUTH_RESPONSE", MockOk("KZ", "auth", simulation));
                    }
                    else
                    {
                        var client = sp.GetRequiredService<IKzEsfClient>();
                        var resp = await client.AuthenticateAsync();
                        await WriteResponseAsync("KZ_AUTH_RESPONSE", resp);
                    }
                }

                if (kzXml is not null)
                {
                    var xmlContent = await File.ReadAllTextAsync(kzXml.FullName);
                    if (simulation)
                    {
                        // [SIMULATION] KZ send
                        await WriteResponseAsync("KZ_SEND_RESPONSE", MockOk("KZ", "send_invoice", simulation));
                    }
                    else
                    {
                        var client = sp.GetRequiredService<IKzEsfClient>();
                        var token = await client.AuthenticateAsync();
                        var resp = await client.SendInvoiceAsync(xmlContent, token);
                        await WriteResponseAsync("KZ_SEND_RESPONSE", resp);
                    }
                }

                if (doUzAuth)
                {
                    if (simulation)
                    {
                        // [SIMULATION] UZ auth
                        await WriteResponseAsync("UZ_AUTH_RESPONSE", MockOk("UZ", "auth", simulation));
                    }
                    else
                    {
                        var client = sp.GetRequiredService<IUzDidoxClient>();
                        var resp = await client.AuthenticateAsync();
                        await WriteResponseAsync("UZ_AUTH_RESPONSE", resp);
                    }
                }

                if (uzJson is not null)
                {
                    var jsonContent = await File.ReadAllTextAsync(uzJson.FullName);
                    if (simulation)
                    {
                        // [SIMULATION] UZ send
                        await WriteResponseAsync("UZ_SEND_RESPONSE", MockOk("UZ", "send_invoice", simulation));
                    }
                    else
                    {
                        var client = sp.GetRequiredService<IUzDidoxClient>();
                        var token = await client.AuthenticateAsync();
                        var resp = await client.SendInvoiceAsync(jsonContent, token);
                        await WriteResponseAsync("UZ_SEND_RESPONSE", resp);
                    }
                }

                Console.WriteLine("✅ Tüm işlemler tamamlandı");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "SandboxRunner çalışma hatası");
                Console.Error.WriteLine(ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }, kzAuth, kzSend, uzAuth, uzSend, outputDirOpt, logDirOpt, envOpt);

        return await root.InvokeAsync(args);
    }
}
