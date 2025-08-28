// Türkçe Açıklama: REST tabanlı entegratörlerde ortak OAuth2 / API Key başlığı üretimi için yardımcı sınıf.
using System.Net.Http.Headers;

namespace Invoice.Infrastructure.Providers.Http;

public static class HttpAuthHelper
{
    // Türkçe: Basit API Key başlığı ekleme (örn: X-API-KEY).
    public static void AddApiKey(HttpRequestMessage req, string headerName, string apiKey)
    {
        req.Headers.TryAddWithoutValidation(headerName, apiKey);
    }

    // Türkçe: Bearer token ekleme (OAuth2).
    public static void AddBearer(HttpRequestMessage req, string accessToken)
    {
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
