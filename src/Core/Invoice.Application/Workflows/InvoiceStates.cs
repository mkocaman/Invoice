// Türkçe: Fatura iş akışı durum sabitleri ve geçiş kuralları
namespace Invoice.Application.Workflows
{
    public static class InvoiceStates
    {
        public const string Draft     = "Draft";
        public const string Submitted = "Submitted";
        public const string Sent      = "Sent";
        public const string Accepted  = "Accepted";
        public const string Rejected  = "Rejected";
        public const string Error     = "Error";
    }

    public static class InvoiceEvents
    {
        public const string Submit   = "Submit";
        public const string Send     = "Send";
        public const string Accept   = "Accept";
        public const string Reject   = "Reject";
        public const string Fail     = "Fail";
    }

    public static class InvoiceFsm
    {
        // Türkçe: Basit geçiş doğrulaması — servisler bu kurala uyar
        public static bool Can(string from, string evt, string to) =>
            (from, evt, to) switch
            {
                (InvoiceStates.Draft,     InvoiceEvents.Submit, InvoiceStates.Submitted) => true,
                (InvoiceStates.Submitted, InvoiceEvents.Send,   InvoiceStates.Sent)      => true,
                (InvoiceStates.Sent,      InvoiceEvents.Accept, InvoiceStates.Accepted)  => true,
                (InvoiceStates.Sent,      InvoiceEvents.Reject, InvoiceStates.Rejected)  => true,
                (_,                       InvoiceEvents.Fail,   InvoiceStates.Error)     => true,
                _ => false
            };
    }
}
