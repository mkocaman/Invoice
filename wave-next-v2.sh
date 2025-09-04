set -euo pipefail

ROOT="$(pwd)"
test -f "Invoice.sln" || { echo "❌ Invoice.sln yok; kök dizinde değiliz: $ROOT"; exit 1; }
echo "==> Çalışma kökü: $ROOT"

echo "1) Temizlik + GitHub Actions'ı kapat"
find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + || true
find . -name "packages.lock.json" -delete || true
if [ -d .github ]; then rm -rf .github; echo "🗑️  .github/ kaldırıldı (Actions kullanılmıyor)"; else echo "✅ .github/ yok"; fi

echo "2) Provider arayüzleri (Supports/SupportsCountry) — hızlı rapor"
PROV_REPORT="$(mktemp)"
grep -R --include="*Provider.cs" -nE "class .*Provider" src | cut -d: -f1 \
| sort -u \
| while read -r F; do
  [ -z "$F" ] && continue
  HAVE_S=$(grep -E "bool\s+Supports\s*\(\s*ProviderType" "$F" || true)
  HAVE_C=$(grep -E "bool\s+SupportsCountry\s*\(" "$F" || true)
  printf " • %-70s  Supports:%s  Country:%s\n" "${F#src/}" "${HAVE_S:+✓}" "${HAVE_C:+✓}"
done | tee "$PROV_REPORT"

echo "3) TR VKN/TCKN helper — yoksa ekle"
HELPER="src/Core/Invoice.Application/Helpers/TurkishIdHelper.cs"
if [ ! -f "$HELPER" ]; then
  mkdir -p "$(dirname "$HELPER")"
  cat > "$HELPER" <<'EOF'
// [KILAVUZ] TR Kimlik Doğrulama — VKN(10) / TCKN(11)
namespace Invoice.Application.Helpers
{
    public static class TurkishIdHelper
    {
        public static bool IsDigits(string? s) => !string.IsNullOrWhiteSpace(s) && s!.All(char.IsDigit);
        public static bool IsVkn(string? s) => IsDigits(s) && s!.Length == 10;
        public static bool IsTckn(string? s) => IsDigits(s) && s!.Length == 11;
    }
}
EOF
  echo "  ➕ eklendi: ${HELPER#src/}"
else
  echo "  ✅ mevcut: ${HELPER#src/}"
fi

echo "4) UBL üretiminde anahtarlar/namespace kontrolü"
REQ_KEYS='InvoiceTypeCode|IssueDate|DocumentCurrencyCode|AccountingSupplierParty|AccountingCustomerParty|TaxTotal|LegalMonetaryTotal|InvoicedQuantity|LineExtensionAmount|PriceAmount|ClassifiedTaxCategory'
grep -R --include="*.cs" -nE "XDocument|XElement" src || true
MISS=$((grep -R --include="*.cs" -nE "$REQ_KEYS" src | wc -l) || true)
[ "$MISS" -gt 0 ] && echo "  ✅ anahtarlar kaynakta görünüyor" || echo "  ⚠️ anahtar ifadeler kaynakta yakalanamadı; UBL helper dosyalarını gözden geçir"

echo "5) Restore + build"
dotnet restore --force --no-cache
dotnet build Invoice.sln -v minimal

echo "6) [SIMULASYON] — Smoke: örnek UBL XML üret"
mkdir -p output
if [ -f tools/Invoice.Smoke/Invoice.Smoke.csproj ]; then
  # TR (çalışması beklenir)
  dotnet run --project tools/Invoice.Smoke/Invoice.Smoke.csproj -- TR FORIBA  > output/TR_FORIBA.xml  || true
  dotnet run --project tools/Invoice.Smoke/Invoice.Smoke.csproj -- TR LOGO    > output/TR_LOGO.xml    || true
  # UZ/KZ (token/network olmayabilir — yine de dosya oluştur)
  dotnet run --project tools/Invoice.Smoke/Invoice.Smoke.csproj -- UZ DIDOX   > output/UZ_DIDOX.xml   || true
  dotnet run --project tools/Invoice.Smoke/Invoice.Smoke.csproj -- UZ FAKTURA > output/UZ_FAKTURA.xml || true
  dotnet run --project tools/Invoice.Smoke/Invoice.Smoke.csproj -- KZ ISESF   > output/KZ_ISESF.xml   || true
else
  echo "ℹ️ tools/Invoice.Smoke yok; smoke adımı atlandı."
fi

echo "7) [SIMULASYON] — İsimlendirme kuralı"
for f in output/*.xml; do
  [ -f "$f" ] || continue
  base="$(basename "$f")"
  [[ "$base" == SIMULASYON_* ]] || mv -f "$f" "output/SIMULASYON_${base}"
done
ls -lh output | sed 's/^/  /'

echo "8) [SIMULASYON] — UBL zorunlu alan kontrolü"
REPORT="docs/STATUS.md"
mkdir -p docs
{
  echo "# Durum Özeti"
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
    for key in "<cbc:InvoiceTypeCode" "<cbc:IssueDate" "<cbc:DocumentCurrencyCode" \
               "<cac:AccountingSupplierParty>" "<cac:AccountingCustomerParty>" \
               "<cac:TaxTotal>" "<cac:LegalMonetaryTotal>"; do
      if grep -q "$key" "$f"; then echo "✓ $key"; else echo "✗ $key"; fi
    done
  done
} > "$REPORT"
echo "📄 Rapor: $REPORT"

echo "== ✔︎ WAVE-NEXT v2 bitti =="
