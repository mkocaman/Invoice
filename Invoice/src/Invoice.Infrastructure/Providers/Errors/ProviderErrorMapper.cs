using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Application.Helpers;

namespace Invoice.Infrastructure.Providers.Errors;

/// <summary>
/// Provider hata kodlarını standart hatalara map'ler
/// </summary>
public static class ProviderErrorMapper
{
    /// <summary>
    /// HTTP status code'unu standart hata tipine map'ler
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="providerCode">Provider'ın kendi hata kodu</param>
    /// <param name="providerMessage">Provider'ın hata mesajı</param>
    /// <returns>Standart hata tipi</returns>
    public static ProviderErrorType MapHttpStatusCode(int statusCode, string? providerCode = null, string? providerMessage = null)
    {
        return statusCode switch
        {
            400 => ProviderErrorType.BadRequest,
            401 => ProviderErrorType.AuthError,
            403 => ProviderErrorType.AuthError,
            404 => ProviderErrorType.NotFound,
            409 => ProviderErrorType.Duplicate,
            422 => ProviderErrorType.ValidationError,
            429 => ProviderErrorType.RateLimited,
            >= 500 => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    /// <summary>
    /// Provider hata kodunu standart hata tipine map'ler
    /// </summary>
    /// <param name="providerKey">Provider anahtarı</param>
    /// <param name="providerCode">Provider hata kodu</param>
    /// <returns>Standart hata tipi</returns>
    public static ProviderErrorType MapProviderErrorCode(string providerKey, string? providerCode)
    {
        if (string.IsNullOrEmpty(providerCode))
            return ProviderErrorType.Unknown;

        var normalizedCode = providerCode.ToLowerInvariant();
        var normalizedProvider = providerKey.ToLowerInvariant();

        return normalizedProvider switch
        {
            "foriba" => MapForibaErrorCode(normalizedCode),
            "logo" => MapLogoErrorCode(normalizedCode),
            "mikro" => MapMikroErrorCode(normalizedCode),
            "uyumsoft" => MapUyumsoftErrorCode(normalizedCode),
            "kolaybi" => MapKolayBiErrorCode(normalizedCode),
            "parasut" => MapParasutErrorCode(normalizedCode),
            "dia" => MapDiaErrorCode(normalizedCode),
            "idea" => MapIdeaErrorCode(normalizedCode),
            "bizimhesap" => MapBizimHesapErrorCode(normalizedCode),
            "netsis" => MapNetsisErrorCode(normalizedCode),
            _ => ProviderErrorType.Unknown
        };
    }

    /// <summary>
    /// Hata tipini InvoiceStatus'e map'ler
    /// </summary>
    /// <param name="errorType">Hata tipi</param>
    /// <returns>InvoiceStatus</returns>
    public static InvoiceStatus MapToInvoiceStatus(ProviderErrorType errorType)
    {
        return errorType switch
        {
            ProviderErrorType.BadRequest => InvoiceStatus.REJECTED,
            ProviderErrorType.AuthError => InvoiceStatus.ERROR,
            ProviderErrorType.NotFound => InvoiceStatus.REJECTED,
            ProviderErrorType.Duplicate => InvoiceStatus.REJECTED,
            ProviderErrorType.ValidationError => InvoiceStatus.REJECTED,
            ProviderErrorType.RateLimited => InvoiceStatus.ERROR,
            ProviderErrorType.UpstreamError => InvoiceStatus.ERROR,
            ProviderErrorType.Unknown => InvoiceStatus.ERROR,
            _ => InvoiceStatus.ERROR
        };
    }

    /// <summary>
    /// Hata mesajını maskeli hale getirir
    /// </summary>
    /// <param name="message">Orijinal mesaj</param>
    /// <param name="providerKey">Provider anahtarı</param>
    /// <returns>Maskeli mesaj</returns>
    public static string MaskErrorMessage(string? message, string providerKey)
    {
        if (string.IsNullOrEmpty(message))
            return "Bilinmeyen hata";

        // API anahtarları ve gizli bilgileri mask'le
        var maskedMessage = MaskingHelper.MaskApiKeys(message);
        maskedMessage = MaskingHelper.MaskTaxNumbers(maskedMessage);
        maskedMessage = MaskingHelper.MaskPhoneNumbers(maskedMessage);

        return maskedMessage;
    }

    // Provider-özel hata kod mapping'leri
    private static ProviderErrorType MapForibaErrorCode(string code)
    {
        return code switch
        {
            "invalid_request" => ProviderErrorType.BadRequest,
            "authentication_failed" => ProviderErrorType.AuthError,
            "rate_limit_exceeded" => ProviderErrorType.RateLimited,
            "validation_error" => ProviderErrorType.ValidationError,
            "duplicate_invoice" => ProviderErrorType.Duplicate,
            "service_unavailable" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapLogoErrorCode(string code)
    {
        return code switch
        {
            "geçersiz_istek" => ProviderErrorType.BadRequest,
            "kimlik_doğrulama_hatası" => ProviderErrorType.AuthError,
            "hız_sınırı_aşıldı" => ProviderErrorType.RateLimited,
            "doğrulama_hatası" => ProviderErrorType.ValidationError,
            "mükerrer_fatura" => ProviderErrorType.Duplicate,
            "servis_kullanılamıyor" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapMikroErrorCode(string code)
    {
        return code switch
        {
            "INVALID_REQUEST" => ProviderErrorType.BadRequest,
            "AUTH_FAILED" => ProviderErrorType.AuthError,
            "RATE_LIMIT" => ProviderErrorType.RateLimited,
            "VALIDATION_ERROR" => ProviderErrorType.ValidationError,
            "DUPLICATE" => ProviderErrorType.Duplicate,
            "SERVICE_ERROR" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapUyumsoftErrorCode(string code)
    {
        return code switch
        {
            "invalid_request" => ProviderErrorType.BadRequest,
            "auth_error" => ProviderErrorType.AuthError,
            "rate_limit" => ProviderErrorType.RateLimited,
            "validation_error" => ProviderErrorType.ValidationError,
            "duplicate" => ProviderErrorType.Duplicate,
            "service_error" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapKolayBiErrorCode(string code)
    {
        return code switch
        {
            "INVALID_REQUEST" => ProviderErrorType.BadRequest,
            "AUTH_ERROR" => ProviderErrorType.AuthError,
            "RATE_LIMIT" => ProviderErrorType.RateLimited,
            "VALIDATION_ERROR" => ProviderErrorType.ValidationError,
            "DUPLICATE" => ProviderErrorType.Duplicate,
            "SERVICE_ERROR" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapParasutErrorCode(string code)
    {
        return code switch
        {
            "invalid_request" => ProviderErrorType.BadRequest,
            "authentication_failed" => ProviderErrorType.AuthError,
            "rate_limit_exceeded" => ProviderErrorType.RateLimited,
            "validation_error" => ProviderErrorType.ValidationError,
            "duplicate_invoice" => ProviderErrorType.Duplicate,
            "service_unavailable" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapDiaErrorCode(string code)
    {
        return code switch
        {
            "INVALID_REQUEST" => ProviderErrorType.BadRequest,
            "AUTH_FAILED" => ProviderErrorType.AuthError,
            "RATE_LIMIT" => ProviderErrorType.RateLimited,
            "VALIDATION_ERROR" => ProviderErrorType.ValidationError,
            "DUPLICATE" => ProviderErrorType.Duplicate,
            "SERVICE_ERROR" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapIdeaErrorCode(string code)
    {
        return code switch
        {
            "invalid_request" => ProviderErrorType.BadRequest,
            "auth_error" => ProviderErrorType.AuthError,
            "rate_limit" => ProviderErrorType.RateLimited,
            "validation_error" => ProviderErrorType.ValidationError,
            "duplicate" => ProviderErrorType.Duplicate,
            "service_error" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapBizimHesapErrorCode(string code)
    {
        return code switch
        {
            "INVALID_REQUEST" => ProviderErrorType.BadRequest,
            "AUTH_ERROR" => ProviderErrorType.AuthError,
            "RATE_LIMIT" => ProviderErrorType.RateLimited,
            "VALIDATION_ERROR" => ProviderErrorType.ValidationError,
            "DUPLICATE" => ProviderErrorType.Duplicate,
            "SERVICE_ERROR" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }

    private static ProviderErrorType MapNetsisErrorCode(string code)
    {
        return code switch
        {
            "invalid_request" => ProviderErrorType.BadRequest,
            "auth_error" => ProviderErrorType.AuthError,
            "rate_limit" => ProviderErrorType.RateLimited,
            "validation_error" => ProviderErrorType.ValidationError,
            "duplicate" => ProviderErrorType.Duplicate,
            "service_error" => ProviderErrorType.UpstreamError,
            _ => ProviderErrorType.Unknown
        };
    }
}

/// <summary>
/// Standart hata tipleri
/// </summary>
public enum ProviderErrorType
{
    /// <summary>
    /// Geçersiz istek (400)
    /// </summary>
    BadRequest,
    
    /// <summary>
    /// Kimlik doğrulama hatası (401/403)
    /// </summary>
    AuthError,
    
    /// <summary>
    /// Bulunamadı (404)
    /// </summary>
    NotFound,
    
    /// <summary>
    /// Mükerrer kayıt (409)
    /// </summary>
    Duplicate,
    
    /// <summary>
    /// Doğrulama hatası (422)
    /// </summary>
    ValidationError,
    
    /// <summary>
    /// Hız sınırı aşıldı (429)
    /// </summary>
    RateLimited,
    
    /// <summary>
    /// Upstream servis hatası (5xx)
    /// </summary>
    UpstreamError,
    
    /// <summary>
    /// Bilinmeyen hata
    /// </summary>
    Unknown
}
