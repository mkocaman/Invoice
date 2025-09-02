# EŞÜ / UBL 2.1 Uyum Kontrol Özeti

## ✅ Zorunlu Alanlar
- **InvoiceTypeCode:** Mevcut (380 - Standart satış faturası)
- **IssueDate:** Mevcut (yyyy-MM-dd formatında)
- **Currency:** Mevcut (TRY/UZS/KZT ülkeye göre)
- **TaxTotal:** Mevcut (cac:TaxTotal/cbc:TaxAmount)
- **LegalMonetaryTotal:** Mevcut (LineExtensionAmount, TaxExclusiveAmount, TaxInclusiveAmount, PayableAmount)

## ✅ Satır Zorunlu Alanları
- **ID:** Mevcut (cbc:ID)
- **InvoicedQuantity@unitCode:** Mevcut (UN/ECE Rec 20: C62=Adet, KGM=Kilogram)
- **LineExtensionAmount:** Mevcut (cbc:LineExtensionAmount)
- **Item/Name:** Mevcut (cac:Item/cbc:Name)
- **Price/Amount:** Mevcut (cac:Price/cbc:PriceAmount)
- **ClassifiedTaxCategory:** Mevcut (ID, Percent, TaxScheme/ID)

## ✅ Ülke Doğrulamaları
- **TR (Türkiye):** VKN/TCKN validasyonu (10/11 hane)
- **UZ (Özbekistan):** INN validasyonu (9 hane)
- **KZ (Kazakistan):** BIN validasyonu (12 hane)

## ✅ Tarih/Format
- **Tarih:** yyyy-MM-dd (CultureInfo.InvariantCulture)
- **Tutar:** CultureInfo.InvariantCulture ile decimal
- **Para Birimi:** TRY (TR), UZS (UZ), KZT (KZ)

## ✅ Namespace
- **Invoice-2:** urn:oasis:names:specification:ubl:schema:xsd:Invoice-2
- **CAC:** urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2
- **CBC:** urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2

## ✅ Simülasyon Üretimi
- **Çıktı:** output/SIMULASYON_*.xml
- **Provider'lar:** TR-FORIBA, TR-LOGO, UZ-Didox, UZ-FakturaUz, KZ-IsEsf
- **Validasyon:** Tüm zorunlu alanlar mevcut

## 📋 Kontrol Tarihi
**Son Kontrol:** 2025-09-02  
**Durum:** ✅ EŞÜ/UBL 2.1 %100 Uyumlu
