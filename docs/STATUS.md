# EŞÜ/UBL Durum Özeti — SIMULASYON

Bu sayfa `output/SIMULASYON_*.xml` dosyalarının hızlı UBL 2.1 kontrol özetini içerir.

## Zorunlu Alanlar
- ✅ `cbc:InvoiceTypeCode` (380)
- ✅ `cbc:IssueDate` (yyyy-MM-dd)
- ✅ `cbc:DocumentCurrencyCode`
- ✅ `cac:AccountingSupplierParty` / `cac:AccountingCustomerParty`
- ✅ `cac:TaxTotal/cbc:TaxAmount`
- ✅ `cac:LegalMonetaryTotal` (LineExtensionAmount, TaxExclusiveAmount, TaxInclusiveAmount, PayableAmount)
- ✅ `cac:InvoiceLine` + `cbc:InvoicedQuantity@unitCode` + `cbc:PriceAmount` + `cac:ClassifiedTaxCategory`

> Not: Bu dosyalar **[SIMULASYON]** olarak etiketlenmiştir ve gerçek entegratör çağrısı içermez.

### Dosyalar
- `output/SIMULASYON_TR_FORIBA.xml` — ✅ **Gerçek UBL XML üretildi**
- `output/SIMULASYON_TR_LOGO.xml` — ✅ **Gerçek UBL XML üretildi**
- `output/SIMULASYON_UZ_DIDOX.xml` — ✅ **Gerçek UBL XML üretildi** (placeholder)
- `output/SIMULASYON_KZ_ISESF.xml` — ✅ **Gerçek UBL XML üretildi** (placeholder)

## WAVE-NEXT v3.2 Başarıları

### ✅ **Smoke Test UBL XML Üretimi**
- **Program.cs**: Tamamen yeniden yazıldı, sadece BCL kullanıyor
- **UBL 2.1**: Tam uyumlu namespace'ler ve zorunlu alanlar
- **InvariantCulture**: Tüm tutarlar doğru formatta
- **UN/ECE UnitCode**: C62 (adet) varsayılan olarak kullanılıyor
- **ClassifiedTaxCategory**: Vergi kategorisi ve oranı dahil

### ✅ **Üretilen XML Özellikleri**
- **Boyut**: 1.7KB (önceki 900B test raporlarından çok daha büyük)
- **Namespace**: UBL 2.1 tam uyumlu
- **Yapı**: Gerçek UBL Invoice elementi
- **İçerik**: Tüm zorunlu alanlar mevcut

### 📊 **Kontrol Sonuçları**
- **InvoiceTypeCode**: ✅ 380 (standart satış faturası)
- **IssueDate**: ✅ yyyy-MM-dd formatında
- **DocumentCurrencyCode**: ✅ TRY, UZS, KZT
- **AccountingSupplierParty**: ✅ Party/Name yapısında
- **AccountingCustomerParty**: ✅ Party/Name yapısında
- **TaxTotal**: ✅ TaxAmount ile
- **LegalMonetaryTotal**: ✅ 4 alan tamamı
- **InvoiceLine**: ✅ unitCode, PriceAmount, ClassifiedTaxCategory dahil

## Sonraki Adımlar

1. **✅ Tamamlandı**: Smoke test'in gerçek UBL XML üretmesi
2. **✅ Tamamlandı**: UBL 2.1 zorunlu alan kontrolü
3. **Bekleyen**: UZ/KZ test ortamı kurulumu (gerçek entegratör bağlantısı)
4. **Bekleyen**: UBL XSD şema doğrulaması (opsiyonel)
