# Entegratör Kontrol Listesi (TR/UZ/KZ)

## 🔍 Provider Arayüzü
- ✅ **Supports/SupportsCountry:** Tüm provider'larda uygulanmış
- ✅ **Partial Classes:** _Generated.Provider.Supports.cs ile otomatik implementasyon
- ✅ **Interface Compliance:** IInvoiceProvider tam uyumlu

## 🆔 Ülke Kimlik Doğrulamaları
- ✅ **TR (Türkiye):** VKN/TCKN regex'leri mevcut
- ✅ **UZ (Özbekistan):** INN 9 hane validasyonu mevcut
- ✅ **KZ (Kazakistan):** BIN 12 hane validasyonu mevcut

## 💰 Para Birimleri
- ✅ **TRY:** TR provider'larında doğru setleniyor
- ✅ **UZS:** UZ provider'larında doğru setleniyor
- ✅ **KZT:** KZ provider'larında doğru setleniyor

## 📏 UnitCode
- ✅ **UN/ECE Rec 20:** C62 (Adet), KGM (Kilogram) kullanılıyor
- ✅ **Provider Bazlı:** Her provider kendi birim kodunu destekliyor

## 🌐 API Uçları
- ✅ **Üretim/Sandbox Ayrımı:** SIMULASYON ile işaretlenmiş yerel üretim
- ✅ **Gerçek API:** UZ/KZ provider'ları gerçek endpoint'lere bağlanmaya çalışıyor
- ✅ **Mock Provider'lar:** TR provider'ları yerel UBL XML üretimi yapıyor

## 📋 UBL Alanları
- ✅ **TaxTotal:** cac:TaxTotal/cbc:TaxAmount eksiksiz
- ✅ **LegalMonetaryTotal:** LineExtensionAmount, TaxExclusiveAmount, TaxInclusiveAmount, PayableAmount
- ✅ **InvoiceLine:** cbc:ID, cbc:InvoicedQuantity@unitCode, cbc:LineExtensionAmount
- ✅ **Item/Price:** cac:Item/cbc:Name, cac:Price/cbc:PriceAmount
- ✅ **TaxCategory:** cac:ClassifiedTaxCategory (ID, Percent, TaxScheme)

## 🧪 Test Durumu
- ✅ **Smoke Test:** Başarılı çalışıyor
- ✅ **UBL XML:** Tüm zorunlu alanlar üretiliyor
- ✅ **Provider Resolution:** Factory üzerinden doğru provider'lar bulunuyor

## 📁 Çıktı Dosyaları
- ✅ **SIMULASYON_*.xml:** output/ klasöründe timestamp'li
- ✅ **Validasyon:** Her dosyada zorunlu alanlar mevcut
- ✅ **Format:** UBL 2.1 namespace'leri doğru

## 🔧 Teknik Detaylar
- ✅ **Namespace:** Invoice-2, CAC, CBC doğru tanımlanmış
- ✅ **Culture:** InvariantCulture ile tarih/tutar formatları
- ✅ **HTML Encode:** WebUtility.HtmlEncode ile güvenlik
- ✅ **Async/Await:** Gereksiz async kaldırıldı, Task.FromResult kullanılıyor

## 📅 Son Güncelleme
**Tarih:** 2025-09-02  
**Durum:** ✅ Tüm kontroller geçildi
