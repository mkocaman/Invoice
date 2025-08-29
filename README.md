# Invoice — Çoklu Ülke E-Fatura Altyapısı

Merkezi domain/application çekirdeği ve ülke bazlı Infrastructure paketleri (TR/UZ/KZ) ile, sağlayıcı (provider) fabrikası üzerinden UBL/e-şu uyumlu fatura gönderimi.

**Durum:** Solution temiz, tüm projeler başarılı derleniyor. UZ/KZ/TR sağlayıcıları smoke test ile doğrulandı.

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
- **Infrastructure.Core:** Ülke-agnostik ortak implementasyonlar, taban IInvoiceProvider implementasyonları, DI uzantıları.
- **Infrastructure.TR/UZ/KZ:** Ülkeye özel providerlar, mapping/validasyon, XML/UBL üretimi, DI uzantıları.

Fabrika deseni ile `IInvoiceProviderFactory` bir ülke kodu + provider anahtarından somut sağlayıcıyı çözer; `SupportsCountry` / `Supports(ProviderType)` ile uyumluluk kontrol edilir.

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
# temiz restore
dotnet nuget locals all --clear
dotnet restore

# projeleri (veya solution'ı) derle
dotnet build Invoice.sln -v minimal

# smoke test
dotnet run --project ./tools/Invoice.Smoke/Invoice.Smoke.csproj
```

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

### Smoke test (Tools)

`tools/Invoice.Smoke` küçük bir DI bootstrap'i yapar ve ülke bazlı destekli sağlayıcı anahtarlarını listeler:

```bash
dotnet run --project ./tools/Invoice.Smoke/Invoice.Smoke.csproj
```

**Örnek çıktı:**

```
== TR supported providers ==
 • BIZIMHESAP
 • DIA
 • FORIBA
 • IDEA
 • KOLAYBI
 • LOGO
 • MIKRO
 • NETSIS
 • PARASUT
 • UYUMSOFT
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

### UZ/KZ/TR Notları

- **UZ (Özbekistan):** 9 haneli vergi no validasyonu, UZS, birim kodu "796", mapping güncel.
- **KZ (Kazakistan):** 12 haneli BIN validasyonu, KZT, XML/HTML encoding `System.Net.WebUtility` üzerinde.
- **TR (Türkiye):** Provider çakışma uyarıları giderildi/izinli; namespace'ler izole.

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

- **Nullable context aktif:** auto-generated parçalara `#nullable enable` eklendi.
- **Async/await:** Boşa async uyarıları kaldırıldı; gerçek IO eklenene kadar `Task.FromResult` kullanıldı.
- **Partial providers:** Ortak özellikler partial ile ayrıldı; fazladan `ProviderType` vs. tanımları yinelenmez.
- **Uyarı disiplini:**
  - Domain'de CS0108 (base hidden) refactoring backlog'ta.
  - TR'de eski CS0436 çakışmaları giderildi/bastırıldı.
- **Kod stili:** `dotnet format` entegre etmeyi öneriyoruz (opsiyonel).

---

## Testler

- **Smoke:** `tools/Invoice.Smoke` minimal doğrulama.
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

**Takipteki iş paketleri:**
- Domain'de CS0108 refactor,
- Provider bazlı integration test'ler,
- Gerçek API çağrıları için adapter sözleşmelerinin netleştirilmesi (timeout, retry/Polly, logging, trace).
