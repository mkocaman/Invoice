// Türkçe Açıklama:
// run-all komutu; akışları tetikler, validator'ı çalıştırır ve raporları üretir.
// Yeni: --format md|html|both, --summary-json <path>, --title "<başlık>"

using Microsoft.Extensions.Configuration;
using System.CommandLine;
using System.Diagnostics;

namespace Invoice.SandboxRunner;

public static class RunAllCommand
{
    public static Command Build(
        Option<DirectoryInfo?> outputDirOpt,
        Option<DirectoryInfo?> logDirOpt,
        Option<string?> envOpt)
    {
        var cmd = new Command("run-all", "Tüm akışı (KZ/UZ auth+send) çalıştırır, validasyon ve rapor üretir.");

        var kzXmlOpt = new Option<FileInfo?>("--kz-xml", "KZ için gönderilecek XML (opsiyonel).");
        var uzJsonOpt = new Option<FileInfo?>("--uz-json", "UZ için gönderilecek JSON (opsiyonel).");
        var pythonPathOpt = new Option<FileInfo?>("--python", "Python yolu (opsiyonel).");
        var validatorPathOpt = new Option<FileInfo?>("--validator", "Validator script yolu (opsiyonel).");
        var formatOpt = new Option<string>("--format", () => "both", "Rapor formatı: md | html | both");
        var jsonOutOpt = new Option<FileInfo?>("--summary-json", "Özet JSON çıktı yolu (opsiyonel).");
        var titleOpt = new Option<string>("--title", () => "WAVE-SANDBOX-CREDENTIALS → TEST v3 — Final Rapor", "Rapor başlığı");

        cmd.AddOption(kzXmlOpt);
        cmd.AddOption(uzJsonOpt);
        cmd.AddOption(outputDirOpt);
        cmd.AddOption(logDirOpt);
        cmd.AddOption(pythonPathOpt);
        cmd.AddOption(validatorPathOpt);
        cmd.AddOption(formatOpt);
        cmd.AddOption(jsonOutOpt);
        cmd.AddOption(titleOpt);
        cmd.AddOption(envOpt);

        cmd.SetHandler(async () =>
        {
            var solutionRoot = PathHelper.FindSolutionRoot();
            var defaultRespDir = Path.Combine(solutionRoot, "output", "responses");
            var defaultLogsDir = Path.Combine(solutionRoot, "logs");
            var outputDir = defaultRespDir;
            var logsDir = defaultLogsDir;

            Directory.CreateDirectory(outputDir);
            Directory.CreateDirectory(logsDir);

            // Konfigürasyonu ConfigLoader ile yükle (varsayılan env: Development)
            var config = ConfigLoader.Load();
            var simulation = config.GetValue<bool>("Sandbox:Simulation");
            var format = "both";
            var title = "WAVE-SANDBOX-CREDENTIALS → TEST v3 — Final Rapor";

            // 0) Preflight (gerçek moddaysak ortamı kontrol et)
            await InvokeSelfAsync(new[] { "preflight" });

            // 1) Akışlar
            await InvokeSelfAsync(new[] { "--kz-auth", "--output-dir", outputDir, "--log-dir", logsDir });
            await InvokeSelfAsync(new[] { "--uz-auth", "--output-dir", outputDir, "--log-dir", logsDir });

            // 2) Validator çalıştır
            var python = FindPython();
            var validator = Path.Combine(solutionRoot, "tools", "response-validator.py");
            if (!File.Exists(validator))
                throw new FileNotFoundException("Validator script bulunamadı", validator);

            var validatorOut = await RunValidatorAsync(python, validator, outputDir);

            // 3) Özet oluştur ve raporla
            var summary = ReportBuilder.BuildSummary(title, outputDir, simulation);

            // JSON özet
            var reportsDir = Path.Combine(solutionRoot, "docs", "reports");
            Directory.CreateDirectory(reportsDir);
            var summaryJsonPath = Path.Combine(reportsDir, "summary.json");
            await File.WriteAllTextAsync(summaryJsonPath, ReportBuilder.ToJson(summary));

            // MD/HTML rapor
            var docsDir = Path.Combine(solutionRoot, "docs");
            Directory.CreateDirectory(docsDir);
            var mdPath = Path.Combine(docsDir, "STATUS_SANDBOX_RESULTS.md");
            var htmlPath = Path.Combine(docsDir, "STATUS_SANDBOX_RESULTS.html");

            if (format is "md" or "both")
            {
                var md = ReportBuilder.ToMarkdown(summary, validatorOut);
                await File.WriteAllTextAsync(mdPath, md);
                Console.WriteLine("[OK] Markdown rapor: " + mdPath);
            }
            if (format is "html" or "both")
            {
                var html = ReportBuilder.ToHtml(summary, validatorOut);
                await File.WriteAllTextAsync(htmlPath, html);
                Console.WriteLine("[OK] HTML rapor: " + htmlPath);
            }

            Console.WriteLine("[OK] Özet JSON: " + summaryJsonPath);
        });

        return cmd;
    }

    private static string FindPython()
    {
        var candidates = new[] { "python3", "python" };
        foreach (var c in candidates)
        {
            try
            {
                var psi = new ProcessStartInfo { FileName = c, Arguments = "--version", RedirectStandardOutput = true, RedirectStandardError = true };
                using var p = Process.Start(psi)!;
                p.WaitForExit(4000);
                if (p.ExitCode == 0) return c;
            }
            catch { }
        }
        return "python3";
    }

    private static async Task<string> RunValidatorAsync(string python, string validatorPy, string outputDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = python,
            Arguments = $"{Quote(validatorPy)} {Quote(outputDir)}",
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var proc = Process.Start(psi)!;
        var stdout = await proc.StandardOutput.ReadToEndAsync();
        var stderr = await proc.StandardError.ReadToEndAsync();
        proc.WaitForExit();
        return $"[validator stdout]\n{stdout}\n[validator stderr]\n{stderr}\n[exitcode] {proc.ExitCode}\n";
    }

    private static async Task InvokeSelfAsync(string[] args)
    {
        var exe = "dotnet";
        var projectDll = typeof(Program).Assembly.Location;
        var psi = new ProcessStartInfo
        {
            FileName = exe,
            Arguments = $"{Quote(projectDll)} {string.Join(' ', args.Select(Quote))}",
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var p = Process.Start(psi)!;
        var o = await p.StandardOutput.ReadToEndAsync();
        var e = await p.StandardError.ReadToEndAsync();
        p.WaitForExit();
        Console.WriteLine(o);
        if (!string.IsNullOrWhiteSpace(e)) Console.Error.WriteLine(e);
        if (p.ExitCode != 0) throw new Exception($"Alt komut hatası (exit {p.ExitCode}): {string.Join(' ', args)}");
    }

    private static string Quote(string s) => s.Contains(' ') ? $"\"{s}\"" : s;
}