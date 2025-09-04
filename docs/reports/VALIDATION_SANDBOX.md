# Sandbox Validation Raporu (KZ/UZ)

## 🎯 **Genel Durum**

Sandbox entegrasyonu için validation katmanları hazırlandı ve test edildi.

## ✅ **Validation Script'leri**

### **KZ XML Validator** (`tools/kz_xsd_validator.py`)
- **Amaç**: KZ ESF XML dosyalarını doğrular
- **Özellikler**:
  - XSD şema desteği (lxml ile)
  - Yapısal doğrulama (XSD olmadan da)
  - BIN uzunluk kontrolü (12 hane)
  - TaxNumber uzunluk kontrolü (12 hane)
  - Gerekli alan kontrolü
  - Ürün detay kontrolü

#### **Gerekli Alanlar**
- `Currency`: Para birimi (KZT)
- `Supplier`: Tedarikçi bilgisi
  - `BIN`: 12 haneli vergi numarası
- `Customer`: Müşteri bilgisi
  - `TaxNumber`: 12 haneli vergi numarası
- `Items`: Ürün listesi
  - `Item`: Her ürün için gerekli alanlar
- `Totals`: Toplam bilgileri

#### **Kullanım**
```bash
python3 tools/kz_xsd_validator.py <xml_file> [xsd_file]
```

### **UZ JSON Validator** (`tools/uz_json_validator.py`)
- **Amaç**: UZ Didox/FakturaUZ JSON dosyalarını doğrular
- **Özellikler**:
  - JSON schema doğrulama
  - TIN uzunluk kontrolü (9 hane)
  - Currency kontrolü (UZS)
  - Gerekli alan kontrolü
  - Ürün detay kontrolü

#### **Gerekli Alanlar**
- `documentType`: Doküman tipi
- `invoiceNumber`: Fatura numarası
- `issueDate`: Fatura tarihi
- `currency`: Para birimi (UZS)
- `supplier`: Tedarikçi bilgisi
  - `tin`: 9 haneli vergi numarası
  - `name`: Şirket adı
- `customer`: Müşteri bilgisi
  - `tin`: 9 haneli vergi numarası
  - `name`: Şirket adı
- `lines`: Ürün listesi
- `totals`: Toplam bilgileri

#### **Kullanım**
```bash
python3 tools/uz_json_validator.py <json_file>
```

## 🔍 **Test Sonuçları**

### **KZ XML Validation**
- ✅ Tüm gerekli alanlar mevcut
- ✅ BIN ve TaxNumber uzunlukları doğru
- ✅ Ürün detayları eksiksiz
- ⚠️ XSD şema dosyası bulunamadı (sadece yapısal doğrulama)

### **UZ JSON Validation**
- ✅ Tüm gerekli alanlar mevcut
- ✅ TIN uzunlukları doğru (9 hane)
- ✅ Currency UZS olarak ayarlandı
- ✅ Ürün detayları eksiksiz
- ✅ Meta bilgiler mevcut

## 📋 **Validation Kuralları**

### **KZ (Kazakhstan)**
- **BIN**: 12 hane (zorunlu)
- **TaxNumber**: 12 hane (zorunlu)
- **Currency**: KZT (zorunlu)
- **UnitCode**: C62 (varsayılan)

### **UZ (Uzbekistan)**
- **TIN**: 9 hane (zorunlu)
- **Currency**: UZS (zorunlu)
- **UnitCode**: C62 (varsayılan)

## 🚀 **Sonraki Adımlar**

1. **XSD Şema Entegrasyonu**
   - ESF.gov.kz'dan resmi XSD şeması indir
   - lxml ile tam şema doğrulaması

2. **JSON Schema Entegrasyonu**
   - Didox/FakturaUZ JSON schema tanımı
   - jsonschema kütüphanesi ile doğrulama

3. **Production Validation**
   - Error handling geliştir
   - Logging ekle
   - Performance monitoring

## 📊 **Mevcut Durum**

- **KZ Validator**: ✅ Hazır ve test edildi
- **UZ Validator**: ✅ Hazır ve test edildi
- **XSD Support**: ⚠️ Şema dosyası gerekli
- **JSON Schema**: ⚠️ Schema tanımı gerekli
- **Production Ready**: 🔄 Geliştiriliyor
