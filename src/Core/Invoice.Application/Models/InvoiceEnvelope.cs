#nullable enable

using System;
using System.Collections.Generic;

namespace Invoice.Application.Models
{
    /// <summary>
    /// Türkçe: Fatura zarfı – EŞÜ (UBL/e-Fatura) klavuzuna uyumlu UBL veya JSON üretimi için temel model.
    /// </summary>
    public partial class InvoiceEnvelope
    {
        // Türkçe: Temel fatura bilgileri
        public string TenantId { get; set; } = default!;
        public string InvoiceNumber { get; set; } = default!;
        public DateTime IssueDate { get; set; } = DateTime.UtcNow; // EŞÜ standardı: IssueDate kullanılır
        public string? Series { get; set; } // Türkçe: Seri (varsa)
        public string Currency { get; set; } = "TRY"; // Türkçe: Varsayılan para birimi (ülkeye göre değişir)
        public decimal TotalAmount { get; set; }

        // Türkçe: Müşteri bilgileri
        public CustomerInfo Customer { get; set; } = new();

        // Türkçe: Kalemler
        public List<InvoiceLineItem> LineItems { get; set; } = new();

        // Türkçe: İsteğe bağlı ham UBL XML içeriği (Base64 değil, düz metin)
        public string? RawUblXml { get; set; }

        // Türkçe: Geriye uyumluluk için InvoiceDate property'si (IssueDate'e yönlendirir)
        [Obsolete("EŞÜ standardına uygunluk için IssueDate kullanın")]
        public DateTime? InvoiceDate 
        { 
            get => IssueDate;
            set => IssueDate = value ?? DateTime.UtcNow;
        }

        // Türkçe: Geriye uyumluluk için eski property'ler
        [Obsolete("Customer property'sini kullanın")]
        public string? CustomerName 
        { 
            get => Customer?.Name;
            set 
            {
                if (Customer == null) Customer = new CustomerInfo();
                Customer.Name = value ?? string.Empty;
            }
        }

        [Obsolete("Customer.TaxNumber property'sini kullanın")]
        public string? CustomerTaxNumber 
        { 
            get => Customer?.TaxNumber;
            set 
            {
                if (Customer == null) Customer = new CustomerInfo();
                Customer.TaxNumber = value;
            }
        }

        [Obsolete("LineItems property'sini kullanın")]
        public List<InvoiceLineItem>? Items 
        { 
            get => LineItems;
            set => LineItems = value ?? new List<InvoiceLineItem>();
        }
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

    /// <summary>Türkçe: Fatura kalem bilgisi - EŞÜ uyumlu</summary>
    public class InvoiceLineItem
    {
        public string Description { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; } // örn: 18 => %18
        public string? UnitCode { get; set; } // UN/ECE Rec 20 kodu (örn: "C62" = Adet)
        
        // Türkçe yorum: Provider'ların beklediği ek alanlar
        public string? Name { get; set; } // Kalem adı (Description ile aynı)
        public decimal Total => Quantity * UnitPrice; // Hesaplanmış toplam
    }
}
