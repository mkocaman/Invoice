# STATUS — Sandbox Validation (KZ/UZ)

## 🎯 **Genel Durum**

Post-cleanup stratejisi başarıyla uygulandı. Artık sandbox entegrasyonu için hazırız.

## ✅ **Tamamlanan İşlemler**

### **1. API Mapping Güncelleme**
- [x] `docs/API-MAPPING_KZ.md` → Sandbox bilgileriyle güncellendi
- [x] `docs/API-MAPPING_UZ.md` → Sandbox bilgileriyle güncellendi
- [x] SIMULASYON etiketleri kaldırıldı
- [x] Gerçek sandbox endpoint'leri eklendi

### **2. Validation Katmanları**
- [x] `tools/kz_xsd_validator.py` → KZ XML şema doğrulama
- [x] `tools/uz_json_validator.py` → UZ JSON schema doğrulama
- [x] XSD şema desteği (lxml ile)
- [x] Yapısal doğrulama (XSD olmadan da)

### **3. Smoke Test Güncelleme**
- [x] `tools/smoke-kz.sh` → Sandbox endpoint'lere bağlanacak
- [x] `tools/smoke-uz.sh` → Sandbox endpoint'lere bağlanacak
- [x] SIMULASYON etiketleri kaldırıldı

## 🔧 **Konfigürasyon Durumu**

### **appsettings.Development.json**
```json
"Sandbox": {
  "Simulation": false,  // Artık false olmalı
  "UZ": {
    "BaseUrl": "https://sandbox.didox.uz",
    "AuthPath": "/api/auth/e-imzo",
    "InvoicePath": "/api/invoices"
  },
  "KZ": {
    "WsdlUrl": "https://esf.gov.kz:8443/esf-web/ws/api1?wsdl",
    "BaseUrl": "https://esf-test.kgd.gov.kz"
  }
}
```

## 📋 **Sonraki Adımlar**

### **1. Gerçek Credentials**
- [ ] KZ sandbox kullanıcı adı/şifre
- [ ] UZ sandbox client ID/secret
- [ ] P12 sertifika dosyaları

### **2. Sandbox Testleri**
- [ ] KZ auth endpoint testi
- [ ] UZ E-IMZO token testi
- [ ] Invoice gönderim testleri
- [ ] Response validation

### **3. Production Hazırlığı**
- [ ] Error handling
- [ ] Retry stratejileri
- [ ] Logging
- [ ] Monitoring

## 🚀 **Test Komutları**

### **KZ XML Validation**
```bash
python3 tools/kz_xsd_validator.py docs/normalized/SIMULASYON_KZ_NATIVE_*.xml
```

### **UZ JSON Validation**
```bash
python3 tools/uz_json_validator.py docs/normalized/SIMULASYON_UZ_NATIVE_*.json
```

### **Smoke Tests**
```bash
./tools/smoke-kz.sh
./tools/smoke-uz.sh
```

## 📊 **Mevcut Dosyalar**

- `docs/API-MAPPING_KZ.md` → Güncel sandbox mapping
- `docs/API-MAPPING_UZ.md` → Güncel sandbox mapping
- `tools/kz_xsd_validator.py` → XML validator
- `tools/uz_json_validator.py` → JSON validator
- `tools/smoke-kz.sh` → KZ sandbox test
- `tools/smoke-uz.sh` → UZ sandbox test

## 🎯 **Hedef**

Artık gerçek sandbox credentials ile test yapabilir ve production'a geçiş için hazırlanabiliriz.
