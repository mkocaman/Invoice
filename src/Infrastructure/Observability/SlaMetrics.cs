// Türkçe: SLA ve işlem metrikleri (Prometheus)
using Prometheus;

namespace Infrastructure.Observability
{
    public static class SlaMetrics
    {
        // Türkçe: Duruma göre sayaç (provider,label'lı)
        public static readonly Counter WorkflowTransitions
            = Metrics.CreateCounter("invoice_workflow_transitions_total", "Durum geçiş sayacı", "from", "to", "provider", "country");

        // Türkçe: Gönderim süresi histogramı
        public static readonly Histogram ProviderSendDuration
            = Metrics.CreateHistogram("invoice_provider_send_duration_seconds", "Sağlayıcı gönderim süreleri", "provider", "country");

        // Türkçe: Hata sayacı (provider bazlı)
        public static readonly Counter ProviderErrors
            = Metrics.CreateCounter("invoice_provider_errors_total", "Sağlayıcı hataları", "provider", "country", "stage");

        // Türkçe: Retry sayacı
        public static readonly Counter SendRetries
            = Metrics.CreateCounter("invoice_send_retries_total", "Gönderim yeniden denemeleri", "provider", "country");
    }
}
