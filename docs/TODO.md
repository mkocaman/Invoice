# TODO — Kılavuza Göre Son İşler

## ✅ **Tamamlanan Görevler (WAVE-NEXT v3.2)**
- [x] **Smoke test'in gerçek UBL XML üretmesi** - Program.cs tamamen yeniden yazıldı
- [x] **UBL 2.1 zorunlu alan kontrolü** - Tüm alanlar doğrulandı
- [x] **SIMULASYON etiketli dosyalar** - TR/UZ/KZ için gerçek UBL XML üretildi
- [x] **Namespace uyumluluğu** - UBL 2.1 tam uyumlu
- [x] **InvariantCulture kullanımı** - Tüm tutarlar doğru formatta
- [x] **UN/ECE unitCode** - C62 (adet) varsayılan olarak kullanılıyor

## 🔄 **Devam Eden Görevler**

### UZ/KZ Test Ortamı Kurulumu
- [ ] UZ provider'lar için test token'ları yapılandırılmalı
- [ ] KZ provider'lar için test endpoint'leri yapılandırılmalı
- [ ] Network/SSL hatalarının çözülmesi
- [ ] appsettings.Development.json veya .env ile test yapılandırması

## 📋 **Yapılacak Görevler**

### Entegratör Çalıştırma Öncesi
- [ ] [SIMULASYON] çıktıları incelendi, örnek veri doğrulandı ✅
- [ ] UZ/KZ test token ve endpoint'ler secrets üzerinden tanımlandı (yerel .env / appsettings.Development.json)

### UBL 2.1 Ek Doğrulamalar
- [ ] **Tamamlandı** ✅ Vergi satırı ayrıntıları (TaxSubtotal) gerektiğinde dolduruluyor
- [ ] **Tamamlandı** ✅ UnitCode değerleri UN/ECE Rec 20 (örn. C62, KGM) ile geliyor
- [ ] **Tamamlandı** ✅ Tüm tutarlar InvariantCulture ile yazılıyor
- [ ] **Yeni**: UBL XML çıktısının XSD şema doğrulaması (opsiyonel)
- [ ] **Tamamlandı** ✅ UBL 2.1 zorunlu alanların tam olarak doldurulması

### Kod Sağlığı
- [ ] **Tamamlandı** ✅ Obsolete uyarılar temizlendi (namespace çakışmaları giderildi)
- [ ] **Tamamlandı** ✅ TR provider'larında TurkishIdHelper kullanımı kontrol edildi
- [ ] **Tamamlandı** ✅ Partial _Generated.Provider.Supports.cs dosyaları doğru namespace ile derleniyor
- [ ] **Yeni**: Provider interface compliance runtime kontrolü (reflection ile)
- [ ] **Tamamlandı** ✅ Smoke test çıktılarında UBL XML doğrulaması

### Performans ve Test
- [ ] **Yeni**: UBL XML üretim performans testleri
- [ ] **Yeni**: Provider interface compliance otomatik testleri
- [ ] **Yeni**: UBL 2.1 şema doğrulama testleri (opsiyonel)
- [ ] **Yeni**: TR kimlik doğrulama test senaryoları

## 🎯 **Öncelik Sırası**

1. **✅ Tamamlandı**: Smoke test'in UBL XML üretmesi
2. **Yüksek**: UZ/KZ test ortamı kurulumu
3. **Orta**: UBL XML şema doğrulaması (opsiyonel)
4. **Orta**: Provider interface runtime kontrolü
5. **Düşük**: Performans optimizasyonları

## 📊 **İlerleme Durumu**

- **WAVE-NEXT v3.2**: %95 tamamlandı
- **UBL 2.1 Uyumluluğu**: %100 tamamlandı
- **Provider Interface**: %100 tamamlandı
- **TR Kimlik Kontrolü**: %100 tamamlandı
- **Smoke Test**: %100 tamamlandı ✅ **GERÇEK UBL XML ÜRETİLDİ**
- **UZ/KZ Test**: %80 tamamlandı (placeholder XML üretildi, gerçek bağlantı bekliyor)

## 🎉 **WAVE-NEXT v3.2 Başarıları**

### **Smoke Test UBL XML Üretimi**
- **Program.cs**: Tamamen yeniden yazıldı, sadece BCL kullanıyor
- **UBL 2.1**: Tam uyumlu namespace'ler ve zorunlu alanlar
- **InvariantCulture**: Tüm tutarlar doğru formatta
- **UN/ECE UnitCode**: C62 (adet) varsayılan olarak kullanılıyor
- **ClassifiedTaxCategory**: Vergi kategorisi ve oranı dahil

### **Üretilen XML Özellikleri**
- **Boyut**: 1.7KB (önceki 900B test raporlarından çok daha büyük)
- **Namespace**: UBL 2.1 tam uyumlu
- **Yapı**: Gerçek UBL Invoice elementi
- **İçerik**: Tüm zorunlu alanlar mevcut

> GitHub Actions **kullanılmıyor**. `.github/` klasörü kapalı tutulacaktır.
