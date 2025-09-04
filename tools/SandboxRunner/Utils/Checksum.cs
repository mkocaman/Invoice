// Türkçe Açıklama:
// Response dosyalarının bütünlüğünü rapor etmek için SHA-256 hesaplayıcı yardımcı sınıf.
// English: Simple SHA-256 helper for integrity reporting.

using System.Security.Cryptography;
using System.Text;

namespace Invoice.SandboxRunner;

public static class Checksum
{
    // Türkçe: Verilen dosyanın SHA-256 heşini hex string olarak döndürür.
    public static string Sha256File(string path)
    {
        using var sha = SHA256.Create();
        using var fs = File.OpenRead(path);
        var hash = sha.ComputeHash(fs);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
