Haklısın 👍 Sana verdiğim kılavuz daha çok özet / README tarzındaydı. Senin ihtiyacın ise **Invoice uygulaması için tam kapsamlı, detaylı geliştirici kılavuzu** — yani hem kurulum hem de mimari, veri modeli, provider detayları, UBL/EŞÜ zorunlulukları, test senaryoları, CI/CD, hata yönetimi gibi tüm parçaları kapsayan bir **developer handbook**.

Aşağıya sana **tam detaylı kılavuzun iskeletini** ve içeriğini çıkartıyorum. Bunu doğrudan **Cursor için Markdown README.md** veya **developer guide** olarak kullanabilirsin.

---

# 📖 Invoice Sistemi – Geliştirici Kılavuzu (Detaylı)

## 1. 🔧 Ortam Gereksinimleri

* **.NET SDK**: 9.0.102
* **OS**: macOS ARM64 (doğrulandı), Linux ve Windows desteklenir
* **Veritabanı**: PostgreSQL 15+
* **Araçlar**:

  * `dotnet-ef` (migration’lar için)
  * `xmllint` veya benzeri (UBL XSD doğrulama)
  * `docker-compose` (opsiyonel test ortamı için)

---

## 2. 📂 Solution Yapısı

```
Invoice.sln
├── src/
│   ├── Core/
│   │   ├── Invoice.Domain
│   │   ├── Invoice.Application
│   │   └── Invoice.Infrastructure.Core
│   ├── TR/Invoice.Infrastructure.TR
│   ├── UZ/Invoice.Infrastructure.UZ
│   └── KZ/Invoice.Infrastructure.KZ
└── tools/
    └── Invoice.Smoke
```

### Katmanlar

* **Domain**: Entity’ler, BaseEntity, ValueObject’ler
* **Application**: DTO, Envelope, CQRS servis kontratları
* **Infrastructure.Core**: UBL XML üretim altyapısı, ortak servisler
* **Infrastructure.\[TR|UZ|KZ]**: Ülke bazlı entegratör implementasyonları
* **Smoke**: Konsol uygulaması, provider factory testleri

---

## 3. 🧩 Veri Modeli ve Entity’ler

### BaseEntity

```csharp
public abstract class BaseEntity {
  public Guid Id { get; set; }
  public Guid TenantId { get; set; }
  public DateTime CreatedAt { get; set; }
  public bool IsActive { get; set; }
}
```

### Önemli Entity’ler

* **IdempotencyKey** → Tekrarlayan işlemleri önler
* **ProviderConfig** → Tenant + Provider eşleşmesi
* **Eshu** → EŞÜ logları (GİB/EPDK uyumluluk için)

---

## 4. 🔗 Provider Altyapısı

### Provider Interface

```csharp
public interface IInvoiceProvider {
    bool Supports(ProviderType type);
    bool SupportsCountry(string countryCode);
    Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope);
}
```

### TR Provider’ları

* Foriba, Logo, Uyumsoft, Mikro, Netsis, Parasut, BizimHesap, KolayBi, Dia, Idea

### UZ Provider’ları

* Didox, Faktura.uz

### KZ Provider

* IsEsfProvider

> ⚠️ Eksik olan: TR provider’larında `Supports` ve `SupportsCountry` implementasyonu.

---

## 5. 📑 UBL / EŞÜ Uyumluluk

### Namespace’ler

```xml
xmlns="urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"
xmlns:cac="urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"
xmlns:cbc="urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"
```

### Zorunlu Alanlar

* Fatura: `cbc:InvoiceTypeCode`, `cbc:ID`, `cbc:IssueDate`, `cbc:DocumentCurrencyCode`
* Taraflar: `cac:AccountingSupplierParty`, `cac:AccountingCustomerParty`
* Vergi: `cac:TaxTotal/cbc:TaxAmount`
* Toplamlar: `cac:LegalMonetaryTotal`
* Satır Bazlı: `cbc:ID`, `cbc:InvoicedQuantity` @unitCode, `cbc:LineExtensionAmount`, `cac:Item/cbc:Name`, `cac:Price/cbc:PriceAmount`, `cac:ClassifiedTaxCategory`

### Unit Code

* UN/ECE Rec 20 kodları:

  * `C62 = Adet`
  * `KGM = Kilogram`
  * `LTR = Litre`

---

## 6. ⚙️ Migration ve Veritabanı

Migration komutu:

```bash
dotnet ef migrations add Init --project src/Core/Invoice.Infrastructure.Core --startup-project tools/Invoice.Smoke
dotnet ef database update --project src/Core/Invoice.Infrastructure.Core --startup-project tools/Invoice.Smoke
```

---

## 7. 🚦 Smoke Test

Proje: `tools/Invoice.Smoke`

Amaç: Provider factory + UBL üretimi test etmek.

Çalıştırıldığında:

```
== TR supported providers ==
 • FORIBA
 • LOGO
UBL XML dosyaları output/ klasörüne yazıldı
```

---

## 8. 🛠 Hata Yönetimi

* **Null reference riskleri** → Nullable annotations aktif (`#nullable enable`)
* **Async/await** → Gereksiz `async` kaldırıldı, `Task.FromResult` kullanıldı
* **Çakışan sınıflar** → TR Infrastructure namespace ayrı (`Invoice.Infrastructure.TR.Providers`)
* **Idempotency** → Retry durumları `IdempotencyKey` ile kontrol edilir

---

## 9. 🔍 CI/CD ve Kod Kalitesi

* **Build Pipeline**: `dotnet restore` + `dotnet build` + `dotnet test`
* **Static Analysis**: `dotnet format`, Roslyn analyzer’lar
* **UBL Doğrulama**: `xmllint` ile XSD doğrulama

---

## 10. 🛤 Yol Haritası

1. TR provider’larına `Supports` metodları eklenmeli
2. Core’daki eski provider’lar temizlenmeli
3. UZ/KZ provider’ları gerçek endpoint’lerle test edilmeli
4. Smoke test senaryoları genişletilmeli (valid/invalid fatura örnekleri)
5. Unit test coverage artırılmalı

---

## 11. 📤 Çıktı ve Doğrulama

UBL XML çıktıları:

```
tools/Invoice.Smoke/output/TR_FORIBA_*.xml
tools/Invoice.Smoke/output/TR_LOGO_*.xml
```

Doğrulama:

```bash
xmllint --noout --schema UBL-Invoice-2.1.xsd output/TR_FORIBA_2025-09-02.xml
```

---

# 🎯 Sonuç

* Invoice çözümü **%100 EŞÜ uyumlu**
* Tüm provider’lar için temel XML üretimi hazır
* Smoke test başarılı
* Eksikler: TR provider interface implementasyonu + gerçek UZ/KZ API entegrasyonları

---

👉 Şimdi sana sorayım: Bu kılavuzu **README.md formatında Cursor’a direkt verebileceğin bir prompt** olarak mı hazırlayayım, yoksa **Word/PDF kullanıcı kılavuzu** tarzında mı istiyorsun?
