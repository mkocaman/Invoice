using System;
using System.Collections.Generic;

namespace Invoice.Application.Models
{
    /// <summary>
    /// Türkçe: Fatura zarfı – UBL veya uygun JSON üretimi için temel model.
    /// </summary>
    public partial class InvoiceEnvelope
    {
        // Türkçe: Temel fatura bilgileri
        public string TenantId { get; set; } = default!;
        public string InvoiceNumber { get; set; } = default!;
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public string? Series { get; set; } // Türkçe: Seri (varsa)
        public string Currency { get; set; } = "TRY"; // Türkçe: Varsayılan tek para birimi
        public decimal TotalAmount { get; set; }

        // Türkçe: Müşteri bilgileri
        public CustomerInfo Customer { get; set; } = new();

        // Türkçe: Kalemler
        public List<InvoiceLineItem> LineItems { get; set; } = new();

        // Türkçe: İsteğe bağlı ham UBL XML içeriği (Base64 değil, düz metin)
        public string? RawUblXml { get; set; }
    }

    /// <summary>Türkçe: Müşteri bilgisi</summary>
    public class CustomerInfo
    {
        public string Name { get; set; } = default!;
        public string? TaxNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? Email { get; set; }
        public string? CountryCode { get; set; }
        public string? AddressLine { get; set; }
    }

    /// <summary>Türkçe: Fatura kalem bilgisi</summary>
    public class InvoiceLineItem
    {
        public string Description { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; } // örn: 18 => %18
        public string? UnitCode { get; set; } // örn: "EA"
        
        // Türkçe yorum: Provider'ların beklediği ek alanlar
        public string? Name { get; set; } // Kalem adı (Description ile aynı)
        public decimal Total => Quantity * UnitPrice; // Hesaplanmış toplam
    }
}
