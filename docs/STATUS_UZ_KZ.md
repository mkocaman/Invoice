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
