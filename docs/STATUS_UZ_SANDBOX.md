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

## 📊 Test Sonuçları

### **✅ Tamamlanan**
- [x] Sandbox konfigürasyonu
- [x] Auth endpoint testi ([SIMULASYON])
- [x] Invoice endpoint testi ([SIMULASYON])
- [x] Token response üretimi
- [x] UZ native payload üretimi

### **⏳ Bekleyen**
- [ ] Gerçek sandbox credentials
- [ ] E-IMZO imza entegrasyonu
- [ ] Gerçek API testleri

## 🚀 Sonraki Adımlar

1. **Gerçek sandbox bilgileri** alınacak
2. **E-IMZO imza** entegrasyonu yapılacak
3. **Gerçek API testleri** çalıştırılacak
4. **Production** konfigürasyonu hazırlanacak

> Bu sayfadaki tüm örnekler **[SIMULASYON]** amaçlıdır.
