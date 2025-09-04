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
    "WsdlUrl": "https://esf.gov.kz:8443/esf-web/ws/api1?wsdl",
    "DefaultCurrency": "KZT",
    "TimeoutSeconds": 60
  }
}
```

## 📊 Test Sonuçları

### **✅ Tamamlanan**
- [x] Sandbox konfigürasyonu
- [x] Auth endpoint testi ([SIMULASYON])
- [x] Invoice send endpoint testi ([SIMULASYON])
- [x] Token response üretimi
- [x] KZ native XML üretimi

### **⏳ Bekleyen**
- [ ] Gerçek sandbox credentials
- [ ] IS ESF SDK entegrasyonu
- [ ] Gerçek API testleri

## 🚀 Sonraki Adımlar

1. **Gerçek sandbox bilgileri** alınacak
2. **IS ESF SDK** entegrasyonu yapılacak
3. **Gerçek API testleri** çalıştırılacak
4. **Production** konfigürasyonu hazırlanacak

## 📋 Notlar

- **KZ IS ESF** XML formatı kullanır
- **BIN numaraları** 12 hane olmalı
- **Currency** KZT olmalı
- **Unit codes** UN/ECE Rec 20 standardında

> Bu sayfadaki tüm örnekler **[SIMULASYON]** amaçlıdır.
