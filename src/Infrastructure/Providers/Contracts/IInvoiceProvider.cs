// Türkçe Açıklama:
// Tüm entegratörler (Foriba, Logo, KolayBi, Mikro, Uyumsoft, eLogo, Paraşüt, Sovos, DİA, Luca) bu arayüzü uygular.
// White-label: Sağlayıcıya özel kod, adapter içinde izole edilir.

namespace Infrastructure.Providers.Contracts;

public record ProviderSendResult(
    bool Success,
    string? ExternalInvoiceNumber,
    string? RawRequest,
    string? RawResponse,
    string? ErrorCode,
    string? ErrorMessage
);

public interface IInvoiceProvider
{
    // Türkçe: Sağlayıcı adı (config eşlemesi için)
    string Name { get; }

    // Türkçe: Kimlik doğrulama / token yenileme (gerekiyorsa no-op)
    Task<bool> AuthenticateAsync(CancellationToken ct = default);

    // Türkçe: Fatura gönderimi (UBL XML/JSON kaynaklarından biri zorunlu)
    Task<ProviderSendResult> SendAsync(
        string invoiceId,
        string? ublXml,
        string? jsonPayload,
        CancellationToken ct = default
    );
}
