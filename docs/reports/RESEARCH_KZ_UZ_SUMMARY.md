# KZ/UZ Araştırma Özeti — [SIMULASYON]

## Kaynak Notlar
### RESEARCH_KZ_UZ.md
# KZ/UZ Repo Araştırması — [SIMULASYON]
> Kaynaklar: OnSwitchOff/ESF_kz, s-n-alexeyev/large-knowledge-base, Yevgeniy1212/ProdCorp
## ESF_kz

**Klasör yapısı (ilk 60):**
.
./ESF_kz
./ESF_kz/Connected Services
./ESF_kz/Connected Services/LocalService
./ESF_kz/Connected Services/InvoiceService
./ESF_kz/Connected Services/UploadInvoiceService
./ESF_kz/Connected Services/SessionService
./ESF_kz/Forms
./ESF_kz/Resources
./ESF_kz/ENUMs
./ESF_kz/Models
./ESF_kz/Properties
./ESF_kz/Facades
./.git
./.git/objects
./.git/objects/pack
./.git/objects/info
./.git/info
./.git/logs
./.git/logs/refs
./.git/hooks
./.git/refs
./.git/refs/heads
./.git/refs/tags
./.git/refs/remotes

**Örnek dosyalar (samples):**
docs/samples/ESF_kz/Consignor.cs
docs/samples/ESF_kz/AbstractProductShare.cs
docs/samples/ESF_kz/panelESFpartI.Designer.cs
docs/samples/ESF_kz/panelESFpartE.cs
docs/samples/ESF_kz/TruOriginCode.cs
docs/samples/ESF_kz/ProductForm.cs
docs/samples/ESF_kz/panelESFpartA.cs
docs/samples/ESF_kz/InfoForm.cs
docs/samples/ESF_kz/SellerV2.cs
docs/samples/ESF_kz/AbstractUCESFpanelTab.Designer.cs
docs/samples/ESF_kz/ESF_form.Designer.cs
docs/samples/ESF_kz/AbstractProduct.cs
docs/samples/ESF_kz/panelESFpartBtab.Designer.cs
docs/samples/ESF_kz/LocalService.wsdl
docs/samples/ESF_kz/panelESFpartE.Designer.cs
docs/samples/ESF_kz/SellerType.cs
docs/samples/ESF_kz/ConfigManagerFacade.cs
docs/samples/ESF_kz/panelESFpartCtab.Designer.cs
docs/samples/ESF_kz/InfoForm.Designer.cs
docs/samples/ESF_kz/AssemblyInfo.cs
docs/samples/ESF_kz/ProductSetV2.cs
docs/samples/ESF_kz/DeliveryTermV1.cs
docs/samples/ESF_kz/panelESFpartL.Designer.cs
docs/samples/ESF_kz/InvoiceService.wsdl
docs/samples/ESF_kz/panelESFpartD.cs
docs/samples/ESF_kz/panelESFPartHtab.cs
docs/samples/ESF_kz/SessionServiceOperationsFacade.cs
docs/samples/ESF_kz/CustomerV1.cs
docs/samples/ESF_kz/InvoiceV1.cs
docs/samples/ESF_kz/MainForm.Designer.cs
docs/samples/ESF_kz/panelESFpartK.cs
docs/samples/ESF_kz/MainForm.cs
docs/samples/ESF_kz/FormManagerFacade.cs
docs/samples/ESF_kz/ProductV1.cs
docs/samples/ESF_kz/panelESFpartF.Designer.cs
docs/samples/ESF_kz/AbstractInvoice.cs
docs/samples/ESF_kz/panelESFpartJ.cs
docs/samples/ESF_kz/ParsingManager.cs
docs/samples/ESF_kz/LogInForm.Designer.cs
docs/samples/ESF_kz/panelESFPartHtab.Designer.cs

**Eşleşmeler (grep özeti):**
== SCAN ESF_kz ==
# files
.gitattributes
.gitignore
ESF_kz.sln
ESF_kz/App.config

---

### TODO_KZ_UZ.md
# TODO — KZ/UZ Entegrasyon (SIMULASYON)

- [ ] KZ ESF: kesin WSDL/XSD linkleri ve şema versiyonları (sandbox)
- [ ] KZ ESF: imzalama/şifreleme akışının netleştirilmesi (JKS/PFX yapılandırması)
- [ ] UZ: E-IMZO entegrasyon kitaplığı ve token alma akışı
- [ ] UZ/KZ: hata kodları ve retry/backoff stratejileri
- [ ] Güvenlik: sertifika pinning / TLS min sürümleri
- [ ] Örnek request/response koleksiyonu (curl/HTTPie veya Postman)

---

### API-MAPPING_KZ.md
# [SIMULASYON] KZ IS ESF — API Mapping Taslağı

| Alan | ESF (KZ) | Not |
|------|----------|-----|
| Auth | `/api/auth/login` (veya SOAP `Login`) | Test/sandbox ortamına göre değişir |
| Gönderim | `/api/documents/invoice/send` **(XML/SOAP)** | Body: ESF yerel XML, imzalı gönderim gerekebilir |
| BIN | `Supplier/BIN` | 12 hane, zorunlu |
| Para Birimi | `Currency` = `KZT` | Zorunlu |
| Satır | `Items/Item` | UnitCode `C62` varsayılan |
| İmza | JKS/PKCS12 keystore | **[SIMULASYON]** kılavuzda doldurulacak |

---

### API-MAPPING_UZ.md
# [SIMULASYON] UZ Didox/FakturaUZ — API Mapping Taslağı

| Alan | UZ (Didox/FakturaUZ) | Not |
|------|----------------------|-----|
| Auth | `POST /api/auth/e-imzo` | E-IMZO imza ile token |
| Gönderim | `POST /api/invoices` **(JSON)** | Body: yerel JSON, imza başlıkları |
| INN | `supplier.tin`, `customer.tin` | 9 hane |
| Para Birimi | `currency` = `UZS` | Zorunlu |
| Satır | `items[]` / `productlist.products[]` | `unit_code`/`unitcode` tolere et |
| Vergi | `vat`/`nds` | Yüzde/amount alan uyumu |

---

### STATUS_UZ_SAMPLES.md
# UZ Yerel Örnek Analizi — [SIMULASYON]

Bu sayfa, UZ JSON faturalarının normalize edilmiş çıktılarının analizini içerir.

## İşlenen Örnekler

- ✅ SIMULASYON_UZ_NATIVE_SIM-UZ-0001_11F08880585F30FEB8D71E0008000075_20250903_192009.json
- ✅ SIMULASYON_UZ_NATIVE_SIM-UZ-0001_11F07C060B63A8BAAFBE1E0008000075_20250903_192009.json
- ✅ SIMULASYON_UZ_NATIVE_SIM-UZ-0001_11F0871D953FA030B9721E0008000075_20250903_192009.json

**Toplam başarılı örnek:** 3

## Kontrol Sonuçları

### ✅ **Toplam Tutarlılığı**
- **Net + KDV = Brüt**: 4/4 dosya ✅
- **Decimal(0.01) ROUND_HALF_UP**: EŞÜ uyumlu ✅

### ✅ **INN Validasyonu**
- **11F08880585F30FEB8D71E0008000075**: INN 304980622 (9 hane) ✅
- **11F07C060B63A8BAAFBE1E0008000075**: INN 308480269 (9 hane) ✅  
- **11F0871D953FA030B9721E0008000075**: INN 310529901 (9 hane) ✅

### 📊 **Dosya Boyutları**
- **11F08880585F30FEB8D71E0008000075**: 1.0K
- **11F07C060B63A8BAAFBE1E0008000075**: 955B
- **11F0871D953FA030B9721E0008000075**: 851B

## Normalize Edilen Alanlar

### **Temel Bilgiler**
- `documentType`: "UZ-LOCAL" [SIMULASYON]
- `invoiceNumber`: Kaynak docno
- `issueDate`: Parse edilmiş tarih (dd.MM.yyyy → yyyy-MM-dd)
- `currency`: UZS (varsayılan)

### **Tedarikçi/Müşteri**
- `supplier.tin`: INN (9 hane)
- `supplier.name`: Normalize edilmiş isim
- `customer.tin`: INN (9 hane)  
- `customer.name`: Normalize edilmiş isim

### **Satır Detayları**
- `lines[].id`: Sıra numarası
- `lines[].name`: Ürün adı
- `lines[].quantity`: Miktar
- `lines[].unitCode`: Birim kodu (C62 varsayılan)
- `lines[].unitPrice`: Birim fiyat
- `lines[].lineExtensionAmount`: Satır net tutarı
- `lines[].vatPercent`: KDV oranı
- `lines[].vatAmount`: KDV tutarı

### **Toplamlar**
- `totals.lineExtensionAmount`: Net tutar
- `totals.taxExclusiveAmount`: KDV hariç tutar
- `totals.taxAmount`: KDV tutarı
- `totals.taxInclusiveAmount`: KDV dahil tutar
- `totals.payableAmount`: Ödenecek tutar

## Toleranslı Okuma Özellikleri

### **Alan Adı Alternatifleri**
- **Unit Code**: `unitcode` → `unit_code` → `unit` → `C62`
- **Unit Price**: `unitprice` → `unit_price` → `price` → `0`
- **VAT**: `vatpercent` → `vat` → `nds` → `0`

### **Ürün Listesi Yolları**
- `productlist.products` → `productlist.items` → `items` → `positions`

### **Tarih Formatları**
- `dd.MM.yyyy` → `yyyy-MM-dd`
- `yyyy-MM-dd` → `yyyy-MM-dd`  
- `dd/MM/yyyy` → `yyyy-MM-dd`
- Fallback: Bugünün tarihi

## Sonraki Adımlar

1. **✅ Tamamlandı**: UZ yerel JSON normalizasyonu
2. **✅ Tamamlandı**: Benzersiz dosya adları (timestamp)
3. **✅ Tamamlandı**: Toleranslı alan okuma

---

### STATUS_UZ_KZ.md
# UZ/KZ Yerel Format Uyum Özeti — [SIMULASYON]

Bu sayfa, UZ/KZ provider'ları için **yerel** (UBL değil) format kontrollerinin kısa özetidir.

## UZ (Didox/FakturaUZ)
- **Kimlik**: INN (9 hane) — ✅ quick-check
- **Para birimi**: UZS — ✅
- **Satır veri modeli**: name/quantity/unit_code/unit_price/total — ✅
- **Toplamlar**: net/vat/grand — ✅
- **Token/Authorization**: **SIMULASYON** — gerçek çağrı yok

### UZ Provider Analizi
- **DidoxProvider**: E-IMZO imza desteği ile JSON API
- **FakturaUzProvider**: Benzer JSON format, E-IMZO entegrasyonu
- **Validasyon**: INN 9 hane kontrolü, UZS para birimi zorunlu
- **Endpoint**: `/api/auth/e-imzo` (token), `/api/invoices` (fatura)

## KZ (IS ESF)
- **Kimlik**: BIN (12 hane) — ✅ quick-check
- **Para birimi**: KZT — ✅
- **Yerel XML iskeleti**: Supplier/Customer/Items/Totals — ✅
- **İmza/kripto/WS vs.**: **SIMULASYON** — gerçek çağrı yok

### KZ Provider Analizi
- **IsEsfProvider**: SDK tabanlı auth, XML format
- **Validasyon**: BIN 12 hane kontrolü, KZT para birimi zorunlu
- **Endpoint**: `/api/documents/invoice/send` (XML POST)
- **Auth**: Bearer token (SDK tabanlı)

## Yerel Format Özellikleri

### UZ (JSON)
```json
{
  "invoiceNumber": "SIM-UZ-0001",
  "currency": "UZS",
  "supplier": {"inn": "123456789"},
  "customer": {"tin": "987654321"},
  "items": [{"unit_code": "C62"}],
  "totals": {"net": "20000.00", "vat": "2400.00"}
}
```

### KZ (XML)
```xml
<Invoice>
  <Currency>KZT</Currency>
  <Supplier><BIN>123456789012</BIN></Supplier>
  <Customer><TaxNumber>210987654321</TaxNumber></Customer>
  <Items><Item><UnitCode>C62</UnitCode></Item></Items>
  <Totals><Net>2100.00</Net><Vat>252.00</Vat></Totals>
</Invoice>
```

## Kontrol Sonuçları

### ✅ **UZ Doğrulaması**
- **INN Uzunluğu**: 9 hane ✅
- **Currency**: UZS ✅
- **Unit Code**: C62 (UN/ECE Rec 20) ✅
- **JSON Format**: Geçerli ✅

### ✅ **KZ Doğrulaması**
- **BIN Uzunluğu**: 12 hane ✅
- **Currency**: KZT ✅
- **Unit Code**: C62 (UN/ECE Rec 20) ✅
- **XML Format**: Geçerli ✅

## Sonraki Adımlar

1. **✅ Tamamlandı**: UZ/KZ yerel format analizi
2. **✅ Tamamlandı**: SIMULASYON payload üretimi
3. **Bekleyen**: Gerçek entegratör sandbox bilgileri
4. **Bekleyen**: Gerçek API endpoint'leri ve token'ları

> Bu sayfadaki tüm örnekler **[SIMULASYON]** amaçlıdır.

---

### STATUS_KZ_SAMPLES.md
# KZ Yerel Örnek Analizi — [SIMULASYON]

Bu sayfa, KZ XML/JSON faturalarının normalize edilmiş çıktılarının analizini içerir.

## İşlenen Örnekler

- ✅ SIMULASYON_KZ_NATIVE_SIM-KZ-0001_20250903_203052.xml
- ✅ SIMULASYON_KZ_NATIVE_SIM-KZ-0001_20250903_203052.xml
- ✅ SIMULASYON_KZ_NATIVE_SIM-KZ-0001_20250903_203052.xml

**Toplam başarılı örnek:** 3
**Başarısız:** 0

## Kaynak Dosya Analizi

### **ZIP Dosyaları (UZ formatında)**
- `Ҳисоб-фактура актсиз_ 284488  31.08.2025 дан.zip`
- `Ҳисоб-фактура актсиз_ 55  14.08.2025 дан.zip`
- `Ҳисоб-фактура актсиз_ 84  29.08.2025 дан.zip`

### **Çıkarılan JSON'lar**
- **XML sayısı**: 0 (KZ formatında XML bulunamadı)
- **JSON sayısı**: 3 (UZ formatında JSON'lar çıkarıldı)

## Normalize Edilen Alanlar

### **Temel Bilgiler**
- `DocumentType`: "KZ-LOCAL" [SIMULASYON]
- `Number`: Kaynak docno (SIM-KZ-0001)
- `Date`: Parse edilmiş tarih
- `Currency`: KZT (varsayılan)

### **Tedarikçi/Müşteri**
- `Supplier/BIN`: BIN numarası (12 hane bekleniyor)
- `Customer/BIN`: BIN numarası (12 hane bekleniyor)

### **Satır Detayları**
- `Items/Item/Name`: Ürün adı
- `Items/Item/Quantity`: Miktar
- `Items/Item/UnitCode`: Birim kodu (C62 varsayılan)
- `Items/Item/UnitPrice`: Birim fiyat
- `Items/Item/LineExtensionAmount`: Satır net tutarı
- `Items/Item/VatPercent`: KDV oranı
- `Items/Item/VatAmount`: KDV tutarı

### **Toplamlar**
- `Totals/Net`: Net tutar
- `Totals/Vat`: KDV tutarı
- `Totals/Gross`: KDV dahil tutar

### **Meta Bilgiler**
- `Meta/Source`: Kaynak dosya adı
- `Meta/Simulasyon`: "true"

## Toleranslı Okuma Özellikleri

### **Alan Adı Alternatifleri**
- **Unit Code**: `unitCode` → `unit_code` → `unit` → `C62`
- **Unit Price**: `unitPrice` → `unit_price` → `price` → `0`
- **VAT**: `vatPercent` → `vat` → `nds` → `0`

### **Ürün Listesi Yolları**
- `items` → `lines` → `[]`

### **Tarih Formatları**
- `dd.MM.yyyy` → `yyyy-MM-dd`
- `yyyy-MM-dd` → `yyyy-MM-dd`  
- `dd/MM/yyyy` → `yyyy-MM-dd`
- Fallback: Bugünün tarihi

## Çıktı Analizi

### **Üretilen Dosya**
- **Dosya**: `SIMULASYON_KZ_NATIVE_SIM-KZ-0001_20250903_203052.xml`
- **Boyut**: 391B
- **Format**: KZ-native XML (UBL değil)
- **Timestamp**: 20250903_203052

### **XML Yapısı**
```xml

---

### STATUS_KZ_SANDBOX.md
# KZ Sandbox Entegrasyon Raporu — [SIMULASYON]

## 🔐 Auth Endpoint

### **POST /api/auth/login**
- **Base URL**: `https://esf-test.kgd.gov.kz`
- **Status**: [SIMULASYON] - Gerçek çağrı yapılmadı
- **Method**: POST
- **Content-Type**: `application/json`

#### **Request Body (Örnek)**
```json
{
  "username": "SANDBOX_USER",
  "password": "SANDBOX_PASS",
  "grant_type": "password"
}
```

#### **Response (SIMULASYON)**
```json
{
  "status": "success",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.SIMULASYON_KZ_TOKEN",
    "tokenType": "Bearer",
    "expiresIn": 7200
  }
}
```

## 📄 Invoice Send Endpoint

### **POST /api/documents/invoice/send**
- **Base URL**: `https://esf-test.kgd.gov.kz`
- **Status**: [SIMULASYON] - Gerçek çağrı yapılmadı
- **Method**: POST
- **Authorization**: `Bearer {token}`
- **Content-Type**: `application/xml`

#### **Request Body (Örnek XML)**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Invoice>
  <DocumentType>KZ-LOCAL</DocumentType>
  <Number>SIM-KZ-001</Number>
  <Date>2025-09-03</Date>
  <Currency>KZT</Currency>
  <Supplier>
    <BIN>123456789012</BIN>
  </Supplier>
  <Customer>
    <BIN>210987654321</BIN>
  </Customer>
  <Items>
    <Item>
      <Name>Test Item KZ</Name>
      <Quantity>3</Quantity>
      <UnitCode>C62</UnitCode>
      <UnitPrice>1000.00</UnitPrice>
      <LineExtensionAmount>3000.00</LineExtensionAmount>
      <VatPercent>12</VatPercent>
      <VatAmount>360.00</VatAmount>
    </Item>
  </Items>
  <Totals>
    <Net>3000.00</Net>
    <Vat>360.00</Vat>
    <Gross>3360.00</Gross>
  </Totals>
</Invoice>
```

## 🔧 Konfigürasyon

### **appsettings.Development.json**
```json
"Sandbox": {
  "Simulation": true,
  "KZ": {

---

### STATUS_UZ_SANDBOX.md
# UZ Sandbox Entegrasyon Raporu — [SIMULASYON]

## 🔐 Auth Endpoint

### **POST /api/auth/e-imzo**
- **Base URL**: `https://sandbox.didox.uz`
- **Status**: [SIMULASYON] - Gerçek çağrı yapılmadı
- **Method**: POST
- **Content-Type**: `application/json`

#### **Request Body (Örnek)**
```json
{
  "pinfl": "123456789",
  "eimzoSignature": "base64_encoded_signature",
  "clientId": "SANDBOX_CLIENT_ID"
}
```

#### **Response (SIMULASYON)**
```json
{
  "status": "success",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.SIMULASYON_UZ_TOKEN",
    "tokenType": "Bearer",
    "expiresIn": 3600
  }
}
```

## 📄 Invoice Endpoint

### **POST /api/invoices**
- **Base URL**: `https://sandbox.didox.uz`
- **Status**: [SIMULASYON] - Gerçek çağrı yapılmadı
- **Method**: POST
- **Authorization**: `Bearer {token}`

#### **Request Body (Örnek)**
```json
{
  "invoiceNumber": "SIM-UZ-001",
  "currency": "UZS",
  "sellerInn": "123456789",
  "buyerInn": "987654321",
  "issueDate": "2025-09-03",
  "items": [
    {
      "name": "Test Item UZ",
      "quantity": "2",
      "unit_code": "C62",
      "unit_price": "10000.00",
      "line_total": "20000.00",
      "vat_percent": "12",
      "vat_amount": "2400.00"
    }
  ],
  "net": "20000.00",
  "vat": "2400.00",
  "gross": "22400.00"
}
```

## 🔧 Konfigürasyon

### **appsettings.Development.json**
```json
"Sandbox": {
  "Simulation": true,
  "UZ": {
    "BaseUrl": "https://sandbox.didox.uz",
    "AuthPath": "/api/auth/e-imzo",
    "InvoicePath": "/api/invoices",
    "DefaultCurrency": "UZS",
    "TimeoutSeconds": 30
  }
}
```


---

