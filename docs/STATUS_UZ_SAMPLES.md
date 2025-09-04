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
4. **Bekleyen**: UZ gerçek API mapping taslağı
5. **Bekleyen**: KZ için benzer normalizasyon

> Bu sayfadaki tüm örnekler **[SIMULASYON]** amaçlıdır.
