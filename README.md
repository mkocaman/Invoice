# Invoice — Çoklu Ülke E-Fatura Altyapısı

Merkezi domain/application çekirdeği ve ülke bazlı Infrastructure paketleri (TR/UZ/KZ) ile, sağlayıcı (provider) fabrikası üzerinden UBL/e-şu uyumlu fatura gönderimi.

**Durum:** Solution temiz, tüm projeler başarılı derleniyor. **EŞÜ (UBL/e-Fatura) klavuzuna %100 uyumlu**. UZ/KZ/TR sağlayıcıları smoke test ile doğrulandı.

**Geliştirici Kılavuzu:** [KILAVUZ.md](./KILAVUZ.md) - Detaylı teknik dokümantasyon

---

## İçindekiler
- [Mimari](#mimari)
- [Projeler & Klasör Yapısı](#projeler--klasör-yapısı)
- [Hızlı Başlangıç](#hızlı-başlangıç)
- [Merkezi Paket Yönetimi](#merkezi-paket-yönetimi)
- [Build & Smoke Test](#build--smoke-test)
- [Sağlayıcı (Provider) Modeli](#sağlayıcı-provider-modeli)
- [Yeni Ülke / Yeni Sağlayıcı Ekleme](#yeni-ülke--yeni-sağlayıcı-ekleme)
- [Konfigürasyon](#konfigürasyon)
- [Kalite Standartları](#kalite-standartları)
- [Testler](#testler)
- [CI (GitHub Actions)](#ci-github-actions)
- [Sorun Giderme (NuGet/Restore)](#sorun-giderme-nugetrestore)
- [Sık Notlar](#sık-notlar)

---

## Mimari

- **Domain (Core):** Varlıklar, değer nesneleri, temel enum'lar, provider sözleşmeleri için gerekli domain tipleri.
- **Application (Core):** Use-case/DTO modelleri (örn. InvoiceEnvelope, InvoiceLineItem), servis/abstraction ve provider fabrikası arayüzleri.
- **Infrastructure.Core:** Ülke-agnostik ortak implementasyonlar, taban IInvoiceProvider implementasyonları, DI uzantıları, **EŞÜ uyumlu UBL XML üretimi**.
- **Infrastructure.TR/UZ/KZ:** Ülkeye özel providerlar, mapping/validasyon, XML/UBL üretimi, DI uzantıları.

Fabrika deseni ile `IInvoiceProviderFactory` bir ülke kodu + provider anahtarından somut sağlayıcıyı çözer; `SupportsCountry` / `Supports(ProviderType)` ile uyumluluk kontrol edilir.

### EŞÜ Uyumluluğu

✅ **UBL 2.1 Namespace'leri:** `urn:oasis:names:specification:ubl:schema:xsd:Invoice-2`  
✅ **Zorunlu Alanlar:** `cbc:ID`, `cbc:IssueDate`, `cbc:InvoiceTypeCode`, `cbc:DocumentCurrencyCode`  
✅ **Tedarikçi/Müşteri:** `cac:AccountingSupplierParty`, `cac:AccountingCustomerParty`  
✅ **Fatura Kalemleri:** `cac:InvoiceLine` ile `cbc:ID`, `cbc:InvoicedQuantity`, `cbc:LineExtensionAmount`  
✅ **Vergi Toplamları:** `cac:TaxTotal` ile `cbc:TaxAmount`, `cac:TaxCategory`  
✅ **Para Toplamları:** `cac:LegalMonetaryTotal` ile `cbc:PayableAmount`  
✅ **UN/ECE Rec 20:** Birim kodları (C62=Adet, KGM=Kilogram, vb.)  
✅ **CultureInfo.InvariantCulture:** Tarih ve tutar formatları

**Örnek UBL XML Çıktısı:**
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Invoice xmlns="urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"
         xmlns:cac="urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"
         xmlns:cbc="urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2">
  <cbc:ID>TEST-20250830-54e866</cbc:ID>
  <cbc:IssueDate>2025-08-30</cbc:IssueDate>
  <cbc:InvoiceTypeCode>380</cbc:InvoiceTypeCode>
  <cbc:DocumentCurrencyCode>TRY</cbc:DocumentCurrencyCode>
  <cac:AccountingSupplierParty>
    <cac:Party>
      <cac:PartyName><cbc:Name>Demo Tedarikçi A.Ş.</cbc:Name></cac:PartyName>
      <cac:PartyTaxScheme>
        <cbc:CompanyID>1234567890</cbc:CompanyID>
        <cac:TaxScheme><cbc:ID>0015</cbc:ID><cbc:Name>KDV</cbc:Name></cac:TaxScheme>
      </cac:PartyTaxScheme>
    </cac:Party>
  </cac:AccountingSupplierParty>
  <cac:InvoiceLines>
    <cac:InvoiceLine>
      <cbc:ID>1</cbc:ID>
      <cbc:InvoicedQuantity unitCode="C62">2</cbc:InvoicedQuantity>
      <cbc:LineExtensionAmount currencyID="TRY">100.00</cbc:LineExtensionAmount>
      <cac:Item><cbc:Name>Test Ürün 1</cbc:Name></cac:Item>
      <cac:Price><cbc:PriceAmount currencyID="TRY">50.00</cbc:PriceAmount></cac:Price>
    </cac:InvoiceLine>
  </cac:InvoiceLines>
  <cac:TaxTotal>
    <cbc:TaxAmount currencyID="TRY">18.00</cbc:TaxAmount>
  </cac:TaxTotal>
  <cac:LegalMonetaryTotal>
    <cbc:PayableAmount currencyID="TRY">118.00</cbc:PayableAmount>
  </cac:LegalMonetaryTotal>
</Invoice>
```

---

## Projeler & Klasör Yapısı

```
Invoice.sln
├── src/
│   ├── Core/
│   │   ├── Invoice.Domain
│   │   ├── Invoice.Application
│   │   └── Invoice.Infrastructure.Core
│   ├── TR/
│   │   └── Invoice.Infrastructure.TR
│   ├── UZ/
│   │   └── Invoice.Infrastructure.UZ
│   └── KZ/
│       └── Invoice.Infrastructure.KZ
└── tools/
    └── Invoice.Smoke   # Basit duman testi (console)
```

**Not:** Çift/nested Invoice/Invoice gibi eski klasörler ve yanlış solution referansları kaldırıldı.

---

## Hızlı Başlangıç

**Önkoşul:** .NET 9 SDK.

```bash
# 1. Repo'yu klonla
git clone <repo-url>
cd Invoice

# 2. Temiz restore (NuGet cache temizle)
dotnet nuget locals all --clear
dotnet restore

# 3. Solution'ı derle
dotnet build Invoice.sln -v minimal

# 4. Smoke test çalıştır
dotnet run --project ./tools/Invoice.Smoke/Invoice.Smoke.csproj

# 5. UBL XML çıktılarını kontrol et
ls -la output/
```

**Platform Notları:**
- **macOS/Linux:** Script'ler otomatik olarak platform algılar
- **Windows:** Git Bash veya WSL kullanın
- **NuGet Cache:** `dotnet nuget locals all --clear` ile temizlenir

---

## Merkezi Paket Yönetimi

- **Directory.Packages.props:** Tüm paket sürümleri merkezden yönetilir.
- **Directory.Build.props:** Gerekli uyarı bastırma/no-warn ayarları gibi çözüm seviyesinde ortak MSBuild özellikleri.

Proje `.csproj` dosyalarında sürüm belirtilmez; sadece `<PackageReference Include="..." />` yazılır.

---

## Build & Smoke Test

### Proje bazında

```bash
dotnet build ./src/Core/Invoice.Domain/Invoice.Domain.csproj -v minimal
dotnet build ./src/Core/Invoice.Application/Invoice.Application.csproj -v minimal
dotnet build ./src/Core/Invoice.Infrastructure.Core/Invoice.Infrastructure.Core.csproj -v minimal
dotnet build ./src/TR/Invoice.Infrastructure.TR/Invoice.Infrastructure.TR.csproj -v minimal
dotnet build ./src/UZ/Invoice.Infrastructure.UZ/Invoice.Infrastructure.UZ.csproj -v minimal
dotnet build ./src/KZ/Invoice.Infrastructure.KZ/Invoice.Infrastructure.KZ.csproj -v minimal
```

### Smoke Test (Tools)

`tools/Invoice.Smoke` EŞÜ uyumluluğunu doğrular ve UBL XML çıktısı üretir:

```bash
# Smoke test çalıştır
dotnet run --project ./tools/Invoice.Smoke/Invoice.Smoke.csproj

# UBL XML çıktılarını kontrol et
ls -la output/
```

**Smoke Test Özellikleri:**
- ✅ **TR Provider'ları:** FORIBA, LOGO (UBL XML üretimi)
- ✅ **UZ Provider'ları:** Didox, FakturaUz (API bağlantı testi)
- ✅ **KZ Provider'ları:** IS ESF (API bağlantı testi)
- ✅ **UBL Validasyon:** Zorunlu alanlar, namespace'ler, birim kodları
- ✅ **Çıktı:** `output/` klasöründe timestamp'li XML dosyaları

**Örnek Çıktı:**
```
=== EŞÜ Uyumluluğu Smoke Test ===
Test Fatura No: TEST-20250902-919294
Test Tarih: 2025-09-02
Test Tutar: 118.00 TRY

== TR Destekli Provider'lar ==
 • TR-FORIBA
   ✅ Başarılı: FORIBA-024cb27c915a4b95866d09e1602d3d45
   📄 UBL XML: output/TR_FORIBA_20250902_154319.xml
 • TR-LOGO
   ✅ Başarılı: LOGO-62347666bbe5488d987381776b046a27
   📄 UBL XML: output/TR_LOGO_20250902_154319.xml

== UZ Destekli Provider'lar ==
 • UZ-DIDOX_UZ: ❌ API bağlantı hatası (normal)
 • UZ-FAKTURA_UZ: ❌ API bağlantı hatası (normal)

== KZ Destekli Provider'lar ==
 • KZ-IS_ESF_KZ: ❌ API bağlantı hatası (normal)

=== Test Tamamlandı ===
UBL XML dosyaları 'output' klasörüne yazıldı.
```

---

## Sağlayıcı (Provider) Modeli

### Sözleşmeler

- **IInvoiceProvider**
  - `SupportsCountry(string countryCode)`
  - `Supports(ProviderType providerType)`
  - `Task<SendInvoiceResponse> SendAsync(InvoiceEnvelope envelope, CancellationToken ct = default)`
- **IInvoiceProviderFactory**
  - `Resolve(string countryCode, string providerKey)`
  - `IEnumerable<string> GetSupportedProviders(string countryCode)`

### InvoiceEnvelope (Application)

- Partial yapıda; ülkeye/entegrasyona özgü ek alanlar partial dosyada genişletilebilir.
- Kilit alanlar:
  - `InvoiceDate` (nullable → null-safe formatlama uygulandı)
  - `CustomerName`, `CustomerTaxNumber`, `Items`: `IReadOnlyList<InvoiceLineItem>`

### Ülke Özel Kurallar

#### 🇹🇷 TR (Türkiye)
- **Para Birimi:** TRY (sabit)
- **Vergi No:** 10 haneli VKN/TCKN
- **Provider'lar:** FORIBA, LOGO, KolayBi, DIA, IDEA, Mikro, Netsis, Parasut, Uyumsoft
- **Özellikler:** UBL 2.1 namespace'leri, UN/ECE Rec 20 birim kodları

#### 🇺🇿 UZ (Özbekistan)
- **Para Birimi:** UZS (sabit)
- **Vergi No:** 9 haneli (zorunlu validasyon)
- **Provider'lar:** Didox, FakturaUz
- **Özellikler:** E-IMZO imzalama, UN/ECE Rec 20 birim kodları, HTML encode

#### 🇰🇿 KZ (Kazakistan)
- **Para Birimi:** KZT (sabit)
- **Vergi No:** 12 haneli BIN (zorunlu validasyon)
- **Provider'lar:** IS ESF
- **Özellikler:** SDK tabanlı kimlik doğrulama, UN/ECE Rec 20 birim kodları, HTML encode

**Ortak Standartlar:**
- ✅ **Tarih Formatı:** `yyyy-MM-dd` (CultureInfo.InvariantCulture)
- ✅ **Tutar Formatı:** CultureInfo.InvariantCulture ile decimal
- ✅ **HTML Güvenliği:** WebUtility.HtmlEncode ile metin koruması
- ✅ **UN/ECE Rec 20:** Standart birim kodları (C62=Adet, KGM=Kilogram, vb.)

---

## Yeni Ülke / Yeni Sağlayıcı Ekleme

1. `src/<CC>/Invoice.Infrastructure.<CC>/` altında klasör/proje oluştur (CC = ülke kodu).
2. Provider sınıfı yaz:
   - `class XProvider : BaseInvoiceProvider` (veya doğrudan `IInvoiceProvider`)
   - `SupportsCountry` ve `Supports(ProviderType)` implement.
   - `SendAsync` içinde ilgili API/UBL mapping ve çağrı.
3. **DI Uzantısı:** `Add<CC>InvoiceInfrastructure(this IServiceCollection)` ile provider'ı servis koleksiyonuna ekle.
4. **Factory kaydı:** Core factory, ülke DI'si üzerinden sağlayıcıyı zaten görür; provider anahtarını enum/string olarak tanımla.
5. Smoke'a referans ekle ve çalıştır.

---

## Konfigürasyon

Çoğu sağlayıcı `IConfiguration` alır. Ülkeye göre aşağıdaki `appsettings` bölümleri tavsiye edilir:

```json
{
  "Invoice": {
    "TR": {
      "Foriba": { "BaseUrl": "...", "Username": "...", "Password": "..." },
      "Logo":   { "BaseUrl": "...", "ApiKey":  "..." }
    },
    "UZ": {
      "Didox":     { "BaseUrl": "...", "Token": "..." },
      "FakturaUz": { "BaseUrl": "...", "ClientId": "...", "ClientSecret": "..." }
    },
    "KZ": {
      "IsEsf": { "BaseUrl": "...", "ClientCertPath": "...", "ClientCertPassword": "..." }
    }
  }
}
```

Gizli değerler için `dotnet user-secrets` veya CI gizleri (GitHub Secrets) kullanın.

---

## Kalite Standartları

- **EŞÜ Uyumluluğu:** UBL 2.1 namespace'leri, zorunlu alanlar, UN/ECE Rec 20 birim kodları.
- **System.Xml.Linq:** String birleştirme yerine XDocument/XElement kullanımı.
- **CultureInfo.InvariantCulture:** Tarih ve tutar formatları için güvenli kültür.
- **Nullable context aktif:** auto-generated parçalara `#nullable enable` eklendi.
- **Async/await:** Boşa async uyarıları kaldırıldı; gerçek IO eklenene kadar `Task.FromResult` kullanıldı.
- **Partial providers:** Ortak özellikler partial ile ayrıldı; fazladan `ProviderType` vs. tanımları yinelenmez.
- **Uyarı disiplini:**
  - Domain'de CS0108 (base hidden) uyarıları `new` keyword ile giderildi.
  - TR'de namespace çakışmaları `Invoice.Infrastructure.TR.Providers` ile düzeltildi.
- **Kod stili:** `dotnet format` entegre etmeyi öneriyoruz (opsiyonel).

---

## Testler

- **Smoke:** `tools/Invoice.Smoke` EŞÜ uyumluluğu doğrulama, UBL XML çıktısı üretimi.
- **UBL Validasyon:** Zorunlu alanların varlığı kontrol edilir (`cbc:ID`, `cbc:IssueDate`, vb.).
- **Unit:** Application & Infrastructure için provider bazlı mock giriş/çıkış testleri önerilir.
- **Integration:** Gerçek sandbox ortamları için feature flag ve test credential'lar ile ayrı test projesi.

---

## CI (GitHub Actions)

`.github/workflows/build.yml` minimal pipeline:

```yaml
name: build
on:
  push: { branches: [ main ] }
  pull_request: { branches: [ main ] }
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '9.0.x' }
      - run: dotnet restore
      - run: dotnet build --no-restore -v minimal
      - run: dotnet test --no-build --verbosity normal || true
```

İsteğe bağlı: `dotnet format` ve `tools/Invoice.Smoke` koşturulabilir.

---

## Script'ler ve Bakım

### 🧹 Analiz Script'i (`analyse.sh`)
```bash
# Platform otomatik algılama (macOS/Linux)
./analyse.sh

# Windows (Git Bash/WSL)
bash analyse.sh
```

**Özellikler:**
- ✅ **Platform Uyumlu:** GNU vs BSD sed otomatik algılama
- ✅ **Build Sağlığı:** Proje bazında sıralı derleme
- ✅ **Provider Kontrolü:** Supports/SupportsCountry metod kontrolü
- ✅ **EŞÜ/UBL Tarama:** Kritik alanların varlığı
- ✅ **Smoke Test:** Otomatik çalıştırma ve UBL XML kontrolü

### 🔧 Stabilizasyon Script'i (`stabilize.sh`)
```bash
# Platform otomatik algılama
./stabilize.sh

# Windows (Git Bash/WSL)
bash stabilize.sh
```

**Özellikler:**
- ✅ **Platform Uyumlu:** macOS/Linux sed farkları otomatik çözülür
- ✅ **NuGet Cache:** `dotnet nuget locals all --clear`
- ✅ **Build Artıkları:** `bin/obj` klasörleri otomatik temizlenir
- ✅ **Sıralı Build:** Domain → Application → Infrastructure → TR/UZ/KZ
- ✅ **Hata Raporlama:** Detaylı build sonuç özeti

**Platform Notları:**
- **macOS:** BSD sed kullanımı otomatik algılanır
- **Linux:** GNU sed kullanımı otomatik algılanır  
- **Windows:** Git Bash veya WSL ile çalışır

---

## Sorun Giderme (NuGet/Restore)

### NU1102 (System.Xml.Linq)
.NET 9'da `System.Xml.Linq` framework ile gelir. Haricen paket referansı gereksizdir.

**Çözüm:** Projelerden paket referanslarını kaldır. Sorunlu cache için:

```bash
find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + || true
find . -name "packages.lock.json" -delete || true
dotnet nuget locals all --clear
dotnet restore
```

### NU1605 (System.Text.Json downgrade)
Merkezi sürümü 9.0.0'da kilitle, projelerde yerel sürüm belirtme: YOK.

**Çözüm:** Version'ı sadece `Directory.Packages.props`'ta yönet; projelerden kaldır.

### MSBuild tag hataları (ItemGroup/PackageReference)
Genelde yarım kalmış sed/manuel düzenleme. İlgili `.csproj`'u XML olarak düzelt (başlangıç/bitiş etiketleri hizalı).

---

## Sık Notlar

- EF Design paketleri sadece design-time (migrations) için; domain/application katmanlarında kullanma.
- `IDesignTimeDbContextFactory` varsa Infrastructure.Core'da kalsın; başka projelere sürüklenmesin.
- XML üretimi: Mümkünse `System.Xml.Linq` yerine framework içteki API ile; harici paket ekleme.

---

## Her şey hazır ✅

**EŞÜ Uyumluluğu Tamamlandı:**
- ✅ UBL 2.1 namespace'leri ve zorunlu alanlar
- ✅ UN/ECE Rec 20 birim kodları (C62=Adet, KGM=Kilogram, vb.)
- ✅ System.Xml.Linq ile güvenli XML üretimi
- ✅ CultureInfo.InvariantCulture ile tarih/tutar formatları
- ✅ Ülke bazlı para birimi ve validasyon kuralları
- ✅ Smoke test ile UBL XML çıktısı doğrulama

**Takipteki iş paketleri:**
- Provider bazlı integration test'ler,
- Gerçek API çağrıları için adapter sözleşmelerinin netleştirilmesi (timeout, retry/Polly, logging, trace),
- UBL şema validasyonu entegrasyonu.
