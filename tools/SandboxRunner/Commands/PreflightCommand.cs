using Microsoft.Extensions.Configuration;
using System.CommandLine;

namespace Invoice.SandboxRunner;

public static class PreflightCommand
{
    public static Command Build(Option<string?> envOpt)
    {
        var cmd = new Command("preflight", "Gerçek mod öncesi ortam değişkenlerini kontrol eder (Simulation=false).");
        cmd.AddOption(envOpt);

        cmd.SetHandler((string? envName) =>
        {
            // Türkçe: Konfigürasyonu tek noktadan yükle
            var cfg = ConfigLoader.Load(envName);

            var simulation = cfg.GetValue<bool>("Sandbox:Simulation");

            // [ÖNEMLİ] Simulation=true ise preflight'ı atla (short-circuit)
            if (simulation)
            {
                Console.WriteLine("[OK] Simulation=true → Preflight atlandı.");
                return;
            }

            // Türkçe: Gerçek mod için örnek zorunlu env anahtarları.
            var required = new[]
            {
                "KZ_USERNAME", "KZ_PASSWORD",
                "UZ_CLIENT_ID", "UZ_CLIENT_SECRET"
            };

            var missing = required.Where(k => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(k))).ToList();
            if (missing.Count > 0)
            {
                Console.Error.WriteLine("❌ Preflight başarısız! Aşağıdaki ortam değişkenleri eksik:");
                foreach (var m in missing)
                    Console.Error.WriteLine("   - " + m);
                Environment.Exit(2);
            }

            Console.WriteLine("[OK] Preflight geçti: tüm zorunlu env değişkenleri mevcut.");
        }, envOpt);

        return cmd;
    }
}