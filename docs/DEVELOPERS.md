# Geliştirici Kılavuzu

## Simulation Etiketleme Standardı

- Kodda simülasyon blokları **[SIMULATION]** yorumu ile açıkça işaretlenir.
- Sadece simülasyona ait metot/sınıflar `SimulationCodeAttribute` ile dekore edilir.
- Log kayıtları Serilog `Enrich.WithProperty("Tag", "SIMULATION")` ile etiketlidir.
- Üretilen JSON response içeriklerine `"simulation": true/false` alanı eklenir.
- **Üretim** testlerinde `Sandbox:Simulation=false` olmalıdır; aksi halde gerçek çağrı yapılmaz.

## Raporlama & Özet JSON
- `run-all` sonrasında:
  - `docs/STATUS_SANDBOX_RESULTS.md` ve `docs/STATUS_SANDBOX_RESULTS.html` üretilir.
  - `docs/reports/summary.json` özet dosyası yazılır (başlık, tarih, simulation, dosya listesi, SHA-256).

## Preflight
- `Simulation=false` iken `run-all` başlamadan önce `preflight` otomatik çağrılır.
- Gerekli ortam değişkenleri yoksa anlaşılır hata ile süreç durdurulur.
