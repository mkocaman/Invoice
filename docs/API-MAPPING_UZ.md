# UZ Didox/FakturaUZ — API Mapping (Sandbox)

| Alan | UZ (Didox/FakturaUZ) | Not |
|------|----------------------|-----|
| Auth | `POST /api/auth/e-imzo` | `https://sandbox.didox.uz/api/auth/e-imzo` |
| Gönderim | `POST /api/invoices` **(JSON)** | Body: yerel JSON, imza başlıkları |
| INN | `supplier.tin`, `customer.tin` | 9 hane |
| Para Birimi | `currency` = `UZS` | Zorunlu |
| Satır | `items[]` / `productlist.products[]` | `unit_code`/`unitcode` tolere et |
| Vergi | `vat`/`nds` | Yüzde/amount alan uyumu |

## 🔐 Sandbox Endpoints

### **Base URL**: `https://sandbox.didox.uz`

#### **Auth Endpoint**
- **POST** `/api/auth/e-imzo`
- **Content-Type**: `application/json`
- **Body**:
```json
{
  "pinfl": "123456789",
  "eimzoSignature": "base64_encoded_signature",
  "clientId": "SANDBOX_CLIENT_ID",
  "clientSecret": "SANDBOX_CLIENT_SECRET"
}
```

#### **Response**
```json
{
  "status": "success",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "refreshToken": "refresh_token_here",
    "scope": "invoice:read invoice:write"
  }
}
```

#### **Invoice Endpoint**
- **POST** `/api/invoices`
- **Authorization**: `Bearer {token}`
- **Content-Type**: `application/json`

#### **Request Body (JSON)**
```json
{
  "invoiceNumber": "INV-UZ-001",
  "currency": "UZS",
  "sellerInn": "123456789",
  "buyerInn": "987654321",
  "issueDate": "2025-09-03",
  "dueDate": "2025-10-03",
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

#### **Response**
```json
{
  "status": "success",
  "data": {
    "invoiceId": "INV-UZ-001-2025-09-03",
    "status": "accepted",
    "message": "Invoice successfully sent to Didox",
    "didoxId": "DIDOX-12345"
  }
}
```

## 📋 Gerekli Alanlar

### **Invoice Header**
- `invoiceNumber`: Fatura numarası
- `currency`: "UZS"
- `sellerInn`: Satıcı INN (9 hane)
- `buyerInn`: Alıcı INN (9 hane)
- `issueDate`: YYYY-MM-DD format
- `dueDate`: Vade tarihi (opsiyonel)

### **Items**
- `name`: Ürün/hizmet adı
- `quantity`: Miktar
- `unit_code`: Birim kodu (C62 = adet)
- `unit_price`: Birim fiyat
- `line_total`: Satır toplamı
- `vat_percent`: KDV yüzdesi
- `vat_amount`: KDV tutarı

### **Totals**
- `net`: Vergisiz toplam
- `vat`: KDV toplamı
- `gross`: Vergili toplam

## 🔧 Konfigürasyon

```json
"UZ": {
  "BaseUrl": "https://sandbox.didox.uz",
  "AuthPath": "/api/auth/e-imzo",
  "InvoicePath": "/api/invoices",
  "DefaultCurrency": "UZS",
  "TimeoutSeconds": 30
}
```

## 📝 Notlar

- Sandbox ortamında test edildi
- E-IMZO imza entegrasyonu gerekli
- Gerçek credentials gerekli
- JSON schema doğrulaması yapılmalı
- Bearer token authentication
- 3600 saniye token geçerlilik süresi
- PINFL (Personal Identification Number) gerekli

