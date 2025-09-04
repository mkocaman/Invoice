# ===== WAVE-NEXT v3 — EŞÜ/UBL Pekiştirme + [SIMULASYON] raporlama =====
# güvenli mod
set -euo pipefail

# 0) Shell gürültüsünü sustur (VSCode zsh RPROMPT hatası)
( unset RPROMPT 2>/dev/null || true )

# 1) Konum & ön kontroller
ROOT="$(pwd)"
test -f "Invoice.sln" || { echo "❌ Invoice.sln yok; kök dizinde değiliz: $ROOT"; exit 1; }
echo "==> Çalışma kökü: $ROOT"
dotnet --info | head -n 12

echo "2) Temizlik"
find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + || true
find . -name "packages.lock.json" -delete || true
dotnet nuget locals all --clear || true

# 3) Merkezi paket yönetimi ve csproj hijyeni hızlı kontrol (yalnız rapor)
echo "3) Paket hijyeni kısa rapor"
echo "   - Directory.Packages.props var mı? $(test -f Directory.Packages.props && echo YES || echo NO)"
echo "   - Directory.Build.props   var mı? $(test -f Directory.Build.props && echo YES || echo NO)"
grep -RIn --include="*.csproj" -E "<PackageReference Include=\"(System.Xml.Linq|System.Text.Json)\"" src || true

# 4) Build
echo "4) Restore + Build"
dotnet restore --force --no-cache
dotnet build Invoice.sln -v minimal

# 5) Provider arayüz uyumu (Supports/SupportsCountry) — hızlı metinsel rapor
echo "5) Provider interface uyum raporu"
PROV_REPORT="$(mktemp)"
grep -R --include="*Provider.cs" -nE "class .*Provider" src | cut -d: -f1 \
| sort -u \
| while read -r F; do
  [ -z "$F" ] && continue
  HAVE_S=$(grep -E "bool\s+Supports\s*\(\s*ProviderType" "$F" || true)
  HAVE_C=$(grep -E "bool\s+SupportsCountry\s*\(" "$F" || true)
  printf " • %-70s  Supports:%s  Country:%s\n" "${F#src/}" "${HAVE_S:+✓}" "${HAVE_C:+✓}"
done | tee "$PROV_REPORT"

# 6) [SIMULASYON] — TR smoke üretimi (varsa tools/Invoice.Smoke)
echo "6) [SIMULASYON] — TR smoke"
mkdir -p output
if [ -f tools/Invoice.Smoke/Invoice.Smoke.csproj ]; then
  dotnet run --project tools/Invoice.Smoke/Invoice.Smoke.csproj -- TR FORIBA  > output/TR_FORIBA.xml  || true
  dotnet run --project tools/Invoice.Smoke/Invoice.Smoke.csproj -- TR LOGO    > output/TR_LOGO.xml    || true
else
  echo "ℹ️ tools/Invoice.Smoke bulunamadı; smoke adımı atlandı."
fi

# 7) [SIMULASYON] — İsimlendirme kuralı
for f in output/*.xml; do
  [ -f "$f" ] || continue
  base="$(basename "$f")"
  [[ "$base" == SIMULASYON_* ]] || mv -f "$f" "output/SIMULASYON_${base}"
done

# 8) UBL 2.1 zorunlu alan & namespace hızlı doğrulama
echo "8) [SIMULASYON] — UBL zorunlu alan kontrolü"
REQ_KEYS=( \
  "<cbc:InvoiceTypeCode" "<cbc:IssueDate" "<cbc:DocumentCurrencyCode" \
  "<cac:AccountingSupplierParty>" "<cac:AccountingCustomerParty>" \
  "<cac:TaxTotal>" "<cac:LegalMonetaryTotal>" \
)
UBL_NS_MAIN='urn:oasis:names:specification:ubl:schema:xsd:Invoice-2'
UBL_NS_CAC='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2'
UBL_NS_CBC='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2'

REPORT="docs/STATUS.md"
mkdir -p docs
{
  echo "# Durum Özeti"
  echo
  echo "## Build"
  echo
  echo '```'
  echo "SDK: $(dotnet --version)"
  echo "ROOT: $ROOT"
  echo '```'
  echo
  echo "## Provider Arayüz Raporu"
  echo
  echo '```'
  cat "$PROV_REPORT"
  echo '```'
  echo
  echo "## [SIMULASYON] UBL XML Zorunlu Alan Kontrolü"
  for f in output/SIMULASYON_*.xml; do
    [ -f "$f" ] || continue
    echo
    echo "### $(basename "$f")"
    # Namespaceler
    if grep -q "$UBL_NS_MAIN" "$f" && grep -q "$UBL_NS_CAC" "$f" && grep -q "$UBL_NS_CBC" "$f"; then
      echo "✓ UBL 2.1 namespace'ler mevcut"
    else
      echo "✗ UBL 2.1 namespace'ler eksik görünüyor"
    fi
    # Anahtarlar
    for key in "${REQ_KEYS[@]}"; do
      if grep -q "$key" "$f"; then echo "✓ $key"; else echo "✗ $key"; fi
    done
  done
} > "$REPORT"

# 9) TODO kontrol listesi (kılavuza göre)
TODO="docs/TODO.md"
{
  echo "# TODO (Kılavuz Uyum / Entegratörler)"
  echo
  echo "## Entegratör Çalıştırma Öncesi"
  echo "- [ ] [SIMULASYON] çıktıları incelendi, örnek veri doğrulandı"
  echo "- [ ] UZ/KZ test token ve endpoint'ler secrets üzerinden tanımlandı (yerel .env / appsettings.Development.json)"
  echo
  echo "## UBL 2.1 Ek Doğrulamalar"
  echo "- [ ] Vergi satırı ayrıntıları (TaxSubtotal) gerektiğinde dolduruluyor"
  echo "- [ ] UnitCode değerleri UN/ECE Rec 20 (örn. C62, KGM) ile geliyor"
  echo "- [ ] Tüm tutarlar InvariantCulture ile yazılıyor"
  echo
  echo "## Kod Sağlığı"
  echo "- [ ] Obsolete uyarılar temizlendi"
  echo "- [ ] TR provider'larında TurkishIdHelper kullanımı kontrol edildi"
  echo "- [ ] Partial _Generated.Provider.Supports.cs dosyaları doğru namespace ile derleniyor"
} > "$TODO"

echo
echo "📄 Raporlar:"
echo " - $REPORT"
echo " - $TODO"
echo
echo "📂 Çıktılar:"
ls -lh output | sed 's/^/  /'
echo
echo "== ✔︎ WAVE-NEXT v3 tamamlandı =="
