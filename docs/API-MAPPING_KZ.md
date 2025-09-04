# KZ IS ESF — API Mapping (Sandbox)

| Alan | ESF (KZ) | Not |
|------|----------|-----|
| Auth | `POST /api/auth/login` | `https://esf-test.kgd.gov.kz/api/auth/login` |
| Gönderim | `POST /api/documents/invoice/send` **(XML/SOAP)** | Body: ESF yerel XML, imzalı gönderim gerekebilir |
| BIN | `Supplier/BIN` | 12 hane, zorunlu |
| Para Birimi | `Currency` = `KZT` | Zorunlu |
| Satır | `Items/Item` | UnitCode `C62` varsayılan |
| İmza | P12 keystore | Gerçek sandbox credentials gerekli |

## 🔐 Sandbox Endpoints

### **Base URL**: `https://esf-test.kgd.gov.kz`

#### **Auth Endpoint**
- **POST** `/api/auth/login`
- **Content-Type**: `application/json`
- **Body**:
```json
{
  "username": "SANDBOX_USER",
  "password": "SANDBOX_PASS",
  "grant_type": "password"
}
```

#### **Response**
```json
{
  "status": "success",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "tokenType": "Bearer",
    "expiresIn": 7200,
    "refreshToken": "refresh_token_here",
    "scope": "document:read document:write invoice:send"
  }
}
```

#### **Invoice Send Endpoint**
- **POST** `/api/documents/invoice/send`
- **Authorization**: `Bearer {token}`
- **Content-Type**: `application/xml`

#### **Request Body (XML)**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Invoice>
  <DocumentType>KZ-LOCAL</DocumentType>
  <Number>INV-001</Number>
  <Date>2025-09-03</Date>
  <Currency>KZT</Currency>
  <Supplier>
    <BIN>123456789012</BIN>
    <Name>Test Supplier KZ</Name>
  </Supplier>
  <Customer>
    <BIN>210987654321</BIN>
    <Name>Test Customer KZ</Name>
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

#### **Response**
```json
{
  "status": "success",
  "data": {
    "invoiceId": "INV-001-2025-09-03",
    "status": "accepted",
    "message": "Invoice successfully sent to ESF"
  }
}
```

## 📋 Gerekli Alanlar

### **Invoice Header**
- `DocumentType`: "KZ-LOCAL"
- `Number`: Invoice number
- `Date`: YYYY-MM-DD format
- `Currency`: "KZT"

### **Supplier/Customer**
- `BIN`: 12 haneli vergi numarası
- `Name`: Şirket adı

### **Items**
- `Name`: Ürün/hizmet adı
- `Quantity`: Miktar
- `UnitCode`: Birim kodu (C62 = adet)
- `UnitPrice`: Birim fiyat
- `LineExtensionAmount`: Satır toplamı
- `VatPercent`: KDV yüzdesi
- `VatAmount`: KDV tutarı

### **Totals**
- `Net`: Vergisiz toplam
- `Vat`: KDV toplamı
- `Gross`: Vergili toplam

## 🔧 Konfigürasyon

```json
"KZ": {
  "BaseUrl": "https://esf-test.kgd.gov.kz",
  "WsdlUrl": "https://esf.gov.kz:8443/esf-web/ws/api1?wsdl",
  "DefaultCurrency": "KZT",
  "TimeoutSeconds": 60
}
```

## 📝 Notlar

- Sandbox ortamında test edildi
- Gerçek credentials gerekli
- P12 sertifika ile imza gerekli
- XML şema doğrulaması yapılmalı
- Bearer token authentication
- 7200 saniye token geçerlilik süresi

