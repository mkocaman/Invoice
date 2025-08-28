// Türkçe yorum: Derleme hatalarını gidermek için InvoiceEnvelope'a eksik alanlar eklendi.
// Bu dosya güvenli: Ana sınıf 'partial' yapıldı ve burada yalnızca nullable property'ler var.
// İhtiyaç oldukça tipler/sözleşme netleştirilip daraltılabilir.

using System;
using System.Collections.Generic;

namespace Invoice.Application.Models
{
    public partial class InvoiceEnvelope
    {
        // Türkçe yorum: Bazı provider'lar bu alanları bekliyor.
        public DateTime? InvoiceDate { get; set; }              // Fatura tarihi (nullable)
        public string?   CustomerName { get; set; }             // Müşteri adı (nullable)
        public string?   CustomerTaxNumber { get; set; }        // Vergi no (nullable)

        // Türkçe yorum: Provider'ların beklediği kalem tipi
        public IReadOnlyList<InvoiceLineItem>? Items { get; set; }       // Kalemler (nullable)
    }
}
