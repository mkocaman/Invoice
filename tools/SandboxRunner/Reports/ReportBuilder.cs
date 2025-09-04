// Türkçe Açıklama:
// Markdown ve HTML rapor üreticisi. Response klasörünü tarar, dosya listesi,
// boyut, SHA-256, simulation bayrağı gibi bilgileri rapora işler.
// English: Builds MD/HTML reports with file table, stats and simulation info.

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Invoice.SandboxRunner;

public sealed class RunSummary
{
    // Türkçe: Özet metrikler
    public required string Title { get; init; }
    public required DateTime GeneratedAt { get; init; }
    public required bool Simulation { get; init; }
    public required string ResponsesDir { get; init; }
    public required int FileCount { get; init; }
    public required List<FileItem> Files { get; init; } = new();
}

public sealed class FileItem
{
    public required string Name { get; init; }
    public required long SizeBytes { get; init; }
    public required string Sha256 { get; init; }
}

public static class ReportBuilder
{
    // Türkçe: Response klasöründen liste/istatistik çıkar.
    public static RunSummary BuildSummary(string title, string responsesDir, bool simulation)
    {
        var di = new DirectoryInfo(responsesDir);
        if (!di.Exists) di.Create();

        var files = di.GetFiles("*.json", SearchOption.TopDirectoryOnly)
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .Select(f => new FileItem
            {
                Name = f.Name,
                SizeBytes = f.Length,
                Sha256 = Checksum.Sha256File(f.FullName)
            })
            .ToList();

        return new RunSummary
        {
            Title = title,
            GeneratedAt = DateTime.UtcNow,
            Simulation = simulation,
            ResponsesDir = di.FullName,
            FileCount = files.Count,
            Files = files
        };
    }

    // Türkçe: Özet JSON içeriği oluştur.
    public static string ToJson(RunSummary s) =>
        JsonSerializer.Serialize(s, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

    // Türkçe: Markdown rapor metnini üret.
    public static string ToMarkdown(RunSummary s, string validatorBlock)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# " + s.Title);
        sb.AppendLine();
        sb.AppendLine($"- Tarih/Saat (UTC): **{s.GeneratedAt:yyyy-MM-dd HH:mm:ss}**");
        sb.AppendLine($"- Simulation Mode: **{(s.Simulation ? "Açık" : "Kapalı")}**");
        sb.AppendLine($"- Response Dizini: **{s.ResponsesDir}**");
        sb.AppendLine($"- Dosya Sayısı: **{s.FileCount}**");
        sb.AppendLine();
        sb.AppendLine("## 📄 Dosyalar");
        sb.AppendLine("| Dosya Adı | Boyut (byte) | SHA-256 |");
        sb.AppendLine("|---|---:|---|");
        foreach (var f in s.Files)
            sb.AppendLine($"| {f.Name} | {f.SizeBytes} | `{f.Sha256}` |");
        sb.AppendLine();
        sb.AppendLine("## ✅ Validasyon Çıktısı");
        sb.AppendLine("```");
        sb.AppendLine((validatorBlock ?? "").Trim());
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("> Not: Simulation açıkken ağ bağımlılıkları devre dışıdır; gerçek testlerde Simulation=false ile çalıştırın.");
        return sb.ToString();
    }

    // Türkçe: Basit HTML şablonu (Markdown yerine düz HTML tablo).
    public static string ToHtml(RunSummary s, string validatorBlock)
    {
        var rows = string.Join("", s.Files.Select(f =>
            $"<tr><td>{f.Name}</td><td style='text-align:right'>{f.SizeBytes}</td><td><code>{f.Sha256}</code></td></tr>"));

        return $@"
<!doctype html>
<html lang=""tr"">
<head>
<meta charset=""utf-8"">
<title>{Escape(s.Title)}</title>
<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
<style>
body {{ font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif; margin: 24px; }}
h1,h2 {{ margin: 0 0 12px 0; }}
table {{ border-collapse: collapse; width: 100%; margin-top: 8px; }}
th, td {{ border: 1px solid #ddd; padding: 8px; }}
th {{ background: #f8f8f8; text-align: left; }}
code {{ background: #f2f2f2; padding: 2px 4px; border-radius: 4px; }}
.badge {{ display: inline-block; padding: 2px 8px; border-radius: 999px; background: {(s.Simulation ? "#fde68a" : "#dcfce7")}; }}
.footer {{ margin-top: 24px; color: #666; font-size: 12px; }}
</style>
</head>
<body>
  <h1>{Escape(s.Title)}</h1>
  <p><strong>Tarih/Saat (UTC):</strong> {s.GeneratedAt:yyyy-MM-dd HH:mm:ss} &nbsp;|&nbsp;
     <strong>Simulation:</strong> <span class=""badge"">{(s.Simulation ? "Açık" : "Kapalı")}</span> &nbsp;|&nbsp;
     <strong>Response:</strong> {Escape(s.ResponsesDir)} &nbsp;|&nbsp;
     <strong>Dosya:</strong> {s.FileCount}</p>

  <h2>Dosyalar</h2>
  <table>
    <thead><tr><th>Dosya Adı</th><th>Boyut (byte)</th><th>SHA-256</th></tr></thead>
    <tbody>{rows}</tbody>
  </table>

  <h2>Validasyon Çıktısı</h2>
  <pre>{Escape(validatorBlock ?? "").Trim()}</pre>

  <div class=""footer"">White-label rapor. Üretimde Simulation=false kullanılmalıdır.</div>
</body>
</html>";
    }

    private static string Escape(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
