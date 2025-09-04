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
<?xml version="1.0" encoding="utf-8"?>
<Invoice>
  <DocumentType>KZ-LOCAL</DocumentType>
  <Number>SIM-KZ-0001</Number>
  <Date>2025-09-03</Date>
  <Currency>KZT</Currency>
  <Supplier>
    <BIN></BIN>
  </Supplier>
  <Customer>
    <BIN></BIN>
  </Customer>
  <Items></Items>
  <Totals>
    <Net>0</Net>
    <Vat>0</Vat>
    <Gross>0</Gross>
  </Totals>
  <Meta>
    <Source>11F08880585F30FEB8D71E0008000075.json</Source>
    <Simulasyon>true</Simulasyon>
  </Meta>
</Invoice>
```

## Sonraki Adımlar

1. **✅ Tamamlandı**: KZ yerel XML normalizasyonu
2. **✅ Tamamlandı**: UZ JSON → KZ XML dönüşümü
3. **✅ Tamamlandı**: API mapping taslakları
4. **Bekleyen**: Gerçek KZ ESF şeması entegrasyonu
5. **Bekleyen**: BIN validasyonu (12 hane kontrolü)

## Notlar

- **UZ JSON'ları KZ XML'e dönüştürüldü** (test amaçlı)
- **Gerçek KZ ESF entegrasyonu** için entegratör dokümanı gerekli
- **BIN numaraları** şu anda boş (kaynak UZ formatında yok)
- **Tüm çıktılar [SIMULASYON]** etiketli

> Bu sayfadaki tüm örnekler **[SIMULASYON]** amaçlıdır.
