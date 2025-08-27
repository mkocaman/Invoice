# 📄 Invoice Projesi

E-Şarj Ünitesi (EŞÜ) fatura yönetim sistemi - .NET 9.0 ile geliştirilmiş modern mikroservis mimarisi.

## 🏗️ Proje Yapısı

```
Invoice/
├── src/
│   ├── Invoice.Api/           # REST API (net9.0)
│   ├── Invoice.Admin/         # Web Admin UI (net9.0)
│   ├── Invoice.Application/   # İş mantığı katmanı (net9.0)
│   ├── Invoice.Domain/        # Domain entities (net9.0)
│   ├── Invoice.Infrastructure/# Veritabanı ve dış servisler (net9.0)
│   └── Invoice.Workers/
│       ├── SendWorker/        # Fatura gönderme worker (net9.0)
│       └── SchedulerWorker/   # Zamanlanmış işler worker (net9.0)
├── tests/
│   ├── Invoice.Tests.Integration/
│   └── Invoice.Tests.Unit/
└── tools/
    ├── schemas/
    └── scripts/
```

## 🚀 Kurulum

### 1. Ortam Değişkenleri
`.env` dosyasını oluşturun veya `appsettings.json` dosyalarını düzenleyin:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=192.168.1.250;Port=5432;Database=invoice_db;Username=postgres;Password=your_password;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Timeout=15"
  }
}
```

### 2. Veritabanı Kurulumu
Uygulama otomatik olarak:
- PostgreSQL'e bağlanır
- `invoice_db` veritabanını oluşturur (yoksa)
- Migration'ları uygular

### 3. Uygulamayı Başlatma

```bash
# API
cd src/Invoice.Api
dotnet run

# Admin UI
cd src/Invoice.Admin
dotnet run

# SendWorker
cd src/Invoice.Workers/SendWorker
dotnet run

# SchedulerWorker
cd src/Invoice.Workers/SchedulerWorker
dotnet run
```

## 🔧 Yapılandırma

### Connection String Standardı
Tüm projelerde `ConnectionStrings:Postgres` anahtarı kullanılır.

### Health Checks
- `/health` - Liveness probe
- `/health/ready` - Readiness probe

### Middleware Sırası
1. **Korelasyon ID**: `X-Correlation-Id` header'ı otomatik oluşturulur
2. **İdempotency**: POST/PUT/PATCH/DELETE için `Idempotency-Key` header'ı zorunlu (DB tabanlı)
3. **WAF**: Header uzunluk, content-type, riskli pattern kontrolü
4. **Rate Limiting**: IP başına 100 istek/dk, burst 20
5. **Routing**: URL yönlendirme
6. **Authentication/Authorization**: Kimlik doğrulama
7. **Controllers**: API endpoint'leri
8. **Health Checks**: Sağlık kontrolü

## 🔌 Entegratörler

### Desteklenen Entegratörler
Sistem 10 farklı e-fatura entegratörünü destekler:

| Entegratör | ProviderKey | Açıklama |
|------------|-------------|----------|
| Foriba | `foriba` | E-Arşiv ve E-Fatura entegrasyonu |
| Logo | `logo` | Logo Tiger3 ERP entegrasyonu |
| Mikro | `mikro` | Mikro ERP entegrasyonu |
| Uyumsoft | `uyumsoft` | Uyumsoft ERP entegrasyonu |
| KolayBi | `kolaybi` | KolayBi ERP entegrasyonu |
| Parasut | `parasut` | Parasut ERP entegrasyonu |
| Dia | `dia` | Dia ERP entegrasyonu |
| Idea | `idea` | Idea ERP entegrasyonu |
| BizimHesap | `bizimhesap` | BizimHesap ERP entegrasyonu |
| Netsis | `netsis` | Netsis ERP entegrasyonu |

### ProviderConfig Alanları
Her entegratör için aşağıdaki konfigürasyon alanları bulunur:

- **Id** (Guid): Benzersiz kimlik
- **TenantId** (string): Çoklu tenant desteği
- **ProviderKey** (string): Entegratör anahtarı
- **ApiBaseUrl** (string): API endpoint URL'i
- **ApiKey** (string): API anahtarı
- **ApiSecret** (string): API gizli anahtarı
- **WebhookSecret** (string): Webhook imza doğrulama anahtarı
- **VknTckn** (string): VKN/TCKN numarası
- **Title** (string): Firma adı
- **BranchCode** (string): Şube kodu (opsiyonel)
- **SignMode** (enum): İmzalama modu
- **TimeoutSec** (int): Timeout süresi
- **RetryCountOverride** (int): Retry sayısı override
- **CircuitTripThreshold** (int): Circuit breaker eşiği
- **IsActive** (bool): Aktif durumu

### Unique Kuralı
`(TenantId, ProviderKey)` kombinasyonu benzersiz olmalıdır.

### İmzalama Modları

#### ProviderSign (Entegratör İmza)
- JSON format ile entegratöre gönderilir
- Entegratör kendi imzalama işlemini yapar
- Dev ortamında mock çağrı kullanılır

#### SelfSign (Kendi İmza)
- UBL XML oluşturulur
- XAdES-BES ile imzalanır
- Dev ortamında MockSigningService kullanılır
- Base64 encode edilerek gönderilir

### Admin Yönetimi
Admin panelinde `/admin/providers` sayfasından:
- Entegratör listeleme
- Yeni entegratör ekleme
- Mevcut entegratör düzenleme
- **Test Connection**: Bağlantı testi
- **Test Send**: Dummy fatura ile gönderim testi

## 🔄 Akış Şeması

```
EŞÜ/ChargeSession → Envelope → (SelfSign→UBL+İmza | ProviderSign→Provider JSON) → Provider Factory/Adapter → Polly retry/circuit → IntegrationLog → Status
```

### Detaylı Akış
1. **EŞÜ/ChargeSession**: Şarj oturumu verileri
2. **Envelope**: Fatura zarfı oluşturma
3. **SignMode Kontrolü**:
   - SelfSign: UBL XML → XAdES-BES imza → Base64
   - ProviderSign: JSON format hazırlama
4. **Provider Factory**: Uygun adapter seçimi
5. **Adapter**: Entegratöre gönderim
6. **Polly**: Retry/circuit breaker
7. **IntegrationLog**: Sonuç loglama
8. **Status**: Fatura durumu güncelleme

## 🧪 Test

### Örnek Curl Komutları

#### Test Connection
```bash
curl -X POST http://localhost:5000/api/admin/providers/{providerId}/test-connection \
  -H "Content-Type: application/json"
```

#### Test Send
```bash
curl -X POST http://localhost:5000/api/admin/providers/{providerId}/test-send \
  -H "Content-Type: application/json"
```

#### Charge Session
```bash
curl -X POST http://localhost:5000/api/invoices/charge-session \
  -H "Idempotency-Key: test-key-123" \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "test-tenant",
    "providerKey": "foriba",
    "customer": {
      "customerId": "CUST001",
      "name": "Test Müşteri",
      "taxNumber": "1234567890"
    },
    "lineItems": [
      {
        "description": "Test Ürün",
        "quantity": 1,
        "unitPrice": 100.00,
        "lineTotal": 100.00,
        "taxRate": 18,
        "taxAmount": 18.00,
        "unit": "adet"
      }
    ],
    "subTotal": 100.00,
    "taxAmount": 18.00,
    "totalAmount": 118.00,
    "eshuAmount": 5.00
  }'
```

### Smoke Test
```bash
# Health check
curl http://localhost:5000/health

# API test (Idempotency-Key gerekli)
curl -X POST http://localhost:5000/api/invoices \
  -H "Idempotency-Key: test-key-123" \
  -H "Content-Type: application/json" \
  -d '{"test": "data"}'

# İdempotency test (aynı anahtar ile ikinci istek)
curl -X POST http://localhost:5000/api/invoices \
  -H "Idempotency-Key: test-key-123" \
  -H "Content-Type: application/json" \
  -d '{"test": "data"}'
# Beklenen: 409 Conflict

# Webhook test (doğru imza)
curl -X POST http://localhost:5000/webhooks/integrators/foriba \
  -H "X-Signature: valid-signature" \
  -H "X-Timestamp: 2024-01-01T12:00:00Z" \
  -H "Content-Type: application/json" \
  -d '{"status": "success"}'

# Admin UI - Kuyruklar
# http://localhost:5001/Queues
```

## 📊 Türk Lirası (TRY) Desteği
- Admin UI'da Türkçe kültür ayarları aktif
- Para birimi formatlaması: `1.234,56 ₺`
- Tarih formatı: `dd.MM.yyyy`

## 🔐 Güvenlik

### PII Maskeleme
- Plaka numaraları: `34***5`
- VKN/TCKN: `123****9`
- Email: `a***b@domain.com`
- Telefon: `555****89`

### Encryption at Rest
- Hassas veriler AES-GCM ile şifrelenir
- Plaka ve VIN alanları otomatik şifrelenir

## 🐛 Sorun Giderme

### PostgreSQL Bağlantı Sorunları
1. PostgreSQL servisinin çalıştığından emin olun
2. Firewall ayarlarını kontrol edin
3. Kullanıcı yetkilerini doğrulayın

### Migration Sorunları
```bash
# Migration'ları sıfırla
dotnet ef database drop --project src/Invoice.Infrastructure --startup-project src/Invoice.Api

# Migration'ları yeniden uygula
dotnet ef database update --project src/Invoice.Infrastructure --startup-project src/Invoice.Api
```

### Design-time Factory Sorunu
`InvoiceDbContextFactory.cs` dosyasının doğru connection string'i okuduğundan emin olun.

### Rate Limiting Sorunları
- **429 Hata**: Rate limit aşıldığında Retry-After header'ını kontrol edin
- **Burst Limit**: Ani yük artışlarında burst limit'e takılabilir

### Webhook İmza Sorunları
- **HMAC Desync**: Timestamp ve body formatının doğru olduğundan emin olun
- **Secret Key**: Webhook secret'ının doğru yapılandırıldığını kontrol edin

### İdempotency Sorunları
- **409 Conflict**: Aynı anahtar tekrar kullanıldığında oluşur
- **Anahtar Süresi**: 24 saat sonra otomatik temizlenir
- **Hash Kontrolü**: İstek gövdesi değişirse farklı hash oluşur

## 📝 Loglama

### Serilog Yapılandırması
- Console output
- File output: 
  - API: `logs/api-.log`
  - Admin: `logs/admin-.log`
  - SendWorker: `logs/sendworker-.log`
  - SchedulerWorker: `logs/scheduler-.log`
- Günlük rotate, 7 gün saklama
- JSON format (Development)
- Korelasyon ID otomatik eklenir

### Log Seviyeleri
- **Development**: Debug
- **Production**: Information
- **Microsoft**: Warning

## 🔄 Worker Servisleri

### SendWorker
- Fatura gönderme işlemleri
- RabbitMQ kuyruğu: `invoice.toSend`
- Retry kuyruğu: `invoice.retry`
- DLQ: `invoice.dlq`

### SchedulerWorker
- Zamanlanmış işler
- EŞÜ aylık özet: `esu.monthly.summary`
- Cron tabanlı tetikleme

## 📋 Enum Değerleri

### InvoiceStatus
- `DRAFT` - Taslak
- `PENDING` - Beklemede
- `SENT` - Gönderildi
- `ERROR` - Hata
- `CANCELLED` - İptal

### ProviderType
- `FORIBA` - Foriba
- `LOGO` - Logo
- `PARASUT` - Parasut
- `BIZIMHESAP` - BizimHesap
- `DIA` - DIA
- `IDEA` - Idea
- `KOLAYBI` - KolayBi
- `MIKRO` - Mikro
- `NETSIS` - Netsis
- `UYUMSOFT` - Uyumsoft

## 🔄 Polly Retry & Circuit Breaker
- **Retry Policy**: 3 deneme (1s, 2s, 5s exponential backoff)
- **Circuit Breaker**: 10 hatada 60 sn açık
- **429 Handling**: Retry-After header'ına uyum
- **503 Fallback**: Circuit açıkken Service Unavailable döner
- **IntegrationLog**: Tüm denemeler ve sonuçlar loglanır

## 🔐 Webhook İmza Doğrulama
- **HMAC-SHA256**: `timestamp.body` formatında
- **Header'lar**: `X-Signature` (base64), `X-Timestamp`
- **Tolerans**: 5 dakika timestamp farkı
- **SecurityLog**: Başarısız doğrulamalar maskeli loglanır

## 🔄 İdempotency Store
- **DB Tabanlı**: `IdempotencyKeys` tablosu
- **Unique Key**: Aynı anahtar tekrar kullanılamaz
- **409 Conflict**: Duplikasyon durumunda hata döner
- **24 Saat**: Anahtar geçerlilik süresi
- **Hash Kontrolü**: İstek gövdesi hash'i (opsiyonel)

## 📊 Kuyruk İzleme
- **Admin UI**: Kuyruklar bölümü (`/Queues`)
- **Retry Queue**: `RetryJobs` tablosu (son 200 kayıt)
- **DLQ**: `OutboxMessages` tablosu (Failed durumundaki mesajlar)
- **PII Maskeleme**: Mesaj gövdesi maskeli gösterim
- **Yeniden Dene**: Manuel retry ve requeue işlemleri

## 🚀 CI Pipeline
- **GitHub Actions**: `.github/workflows/dotnet-ci.yml`
- **OS**: ubuntu-latest
- **.NET**: 9.0.x
- **Cache**: NuGet packages
- **Test Results**: Artifact olarak saklanır
- **Koşullu Integration Test**: `CI_RUN_INTEGRATION=true` ile çalıştırılır

## 🏷️ Versiyon
- **.NET**: 9.0
- **Entity Framework**: 9.0.8
- **PostgreSQL**: 9.0.4
- **MassTransit**: 8.5.2
- **Serilog**: 9.0.0
- **Polly**: 8.3.0
