#!/usr/bin/env bash
set -euo pipefail

# Platform uyumluluğu için sed komutunu algıla
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS: BSD sed
    SED_CMD="sed"
    SED_INPLACE="-i ''"
    echo "🌍 Platform: macOS (BSD sed)"
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
    # Linux: GNU sed
    SED_CMD="sed"
    SED_INPLACE="-i"
    echo "🌍 Platform: Linux (GNU sed)"
else
    # Windows veya diğer: varsayılan
    SED_CMD="sed"
    SED_INPLACE="-i"
    echo "🌍 Platform: $OSTYPE (varsayılan sed)"
fi

echo "===== 🧭 Başlangıç | $(date) ====="
root="$(pwd)"; echo "ROOT: $root"

echo
echo "===== 🧩 .NET ortam özeti ====="
dotnet --info | sed -n '1,40p' || true

echo
echo "===== 🗂️ Solution / Proje yapısı ====="
sln="Invoice.sln"
if [ -f "$sln" ]; then
  echo "✅ Solution bulundu: $sln"
  dotnet sln list || true
else
  echo "❌ Invoice.sln bulunamadı (pwd: $(pwd))"
fi

echo
echo "===== 🧼 İç içe repo kalıntısı kontrolü (Invoice/Invoice) ====="
if [ -d "Invoice/Invoice" ]; then
  echo "⚠️  Invoice/Invoice/ dizini VAR (eski kalıntı)."
else
  echo "✅ İç içe repo kalıntısı yok."
fi

echo
echo "===== 📦 Merkezi paket yönetimi dosyaları ====="
for f in Directory.Packages.props Directory.Build.props; do
  if [ -f "$f" ]; then
    echo "— $f VAR (ilk 80 satır):"
    awk 'NR<=80' "$f"
  else
    echo "— $f yok"
  fi
done

echo
echo "===== 🔍 csproj hızlı envanter (TF + PaketReferans) ====="
find ./src -name "*.csproj" -maxdepth 6 -print0 | while IFS= read -r -d '' p; do
  echo "— $p"
  awk '/<TargetFramework>/{print "   TF: "$0} /<PackageReference /{print "   PR: "$0}' "$p" | sed 's/^[[:space:]]\+//'
done

echo
echo "===== 🧪 Sıralı Build Sağlık Kontrolü (tek tek) ====="
function try_build() {
  local proj="$1"
  if [ -f "$proj" ]; then
    echo "---- building: $proj"
    if dotnet build "$proj" -v minimal; then
      echo "✅ OK: $proj"
    else
      echo "❌ FAIL: $proj"
    fi
  fi
}
try_build "./src/Core/Invoice.Domain/Invoice.Domain.csproj"
try_build "./src/Core/Invoice.Application/Invoice.Application.csproj"
try_build "./src/Core/Invoice.Infrastructure.Core/Invoice.Infrastructure.Core.csproj"
try_build "./src/TR/Invoice.Infrastructure.TR/Invoice.Infrastructure.TR.csproj"
try_build "./src/UZ/Invoice.Infrastructure.UZ/Invoice.Infrastructure.UZ.csproj"
try_build "./src/KZ/Invoice.Infrastructure.KZ/Invoice.Infrastructure.KZ.csproj"

echo
echo "===== 🧱 Solution genel restore/build ====="
if [ -f "$sln" ]; then
  dotnet restore --force --no-cache || true
  dotnet build "$sln" -v minimal || true
fi

echo
echo "===== 🌍 Provider envanteri ve interface uygunluğu ====="
miss_any=0
while IFS= read -r -d '' f; do
  prov="$(basename "$f")"
  supProv="$(grep -c 'Supports(ProviderType' "$f" || true)"
  supCountry="$(grep -c 'SupportsCountry' "$f" || true)"
  echo "• $f  | Supports(ProviderType): $supProv | SupportsCountry: $supCountry"
  if [ "$supProv" -eq 0 ] || [ "$supCountry" -eq 0 ]; then miss_any=1; fi
done < <(find ./src -type f -name "*Provider.cs" -print0)
if [ "$miss_any" -eq 0 ]; then
  echo "✅ Tüm provider dosyalarında Supports / SupportsCountry mevcut görünüyor."
else
  echo "⚠️  En az bir provider dosyasında eksik metot var (yukarıdaki satırlara bak)."
fi

echo
echo "===== 🧾 EŞÜ/UBL kritik alan taraması (kaynakta) ====="
# Text bazlı hızlı doğrulama; asıl doğrulama smoke XML'leri ile
grep -R --include="*Provider*.cs" -nE \
'InvoiceTypeCode|TaxTotal|LegalMonetaryTotal|InvoicedQuantity|PriceAmount|ClassifiedTaxCategory|IssueDate' ./src || true

echo
echo "===== 🧯 Uyarı/Anti-Pattern taraması ====="
echo "— Generated partial için #nullable etkin mi?"
grep -n "InvoiceEnvelope\.Partial\.Generated\.cs" -r ./src || true
grep -n "^#nullable" -r ./src/Core/Invoice.Application/Models || true
echo "— TR'de CS0436 NoWarn var mı?"
grep -n "NoWarn" -r ./src/TR/Invoice.Infrastructure.TR/*.csproj || echo "NoWarn bulunamadı."

echo
echo "===== 🚬 Smoke projesi (varsa) ====="
if [ -f "./tools/Invoice.Smoke/Invoice.Smoke.csproj" ]; then
  echo "Smoke bulundu, çalıştırılıyor..."
  mkdir -p output
  if dotnet run --project ./tools/Invoice.Smoke/Invoice.Smoke.csproj --no-build; then
    echo "✅ Smoke RUN OK"
  else
    echo "⚠️  Smoke RUN hata verdi."
  fi
  echo "— output/ UBL XML listesi:"
  ls -1 output 2>/dev/null || echo "(dosya yok)"
fi

echo
echo "===== 📌 Nerede Kaldık / Özet ====="
cat <<'EOF'
• Build: Projeler tek tek ve solution genel sonuçları yukarıda.
• Provider uygunluğu: Supports(ProviderType) / SupportsCountry bulundu/bulunmadı raporu verildi.
• EŞÜ/UBL kritik alanları: kaynak kodda hızlı tarama yapıldı (InvoiceTypeCode, TaxTotal, LegalMonetaryTotal, InvoicedQuantity, PriceAmount, ClassifiedTaxCategory, IssueDate).
• Nullable/#nullable ve TR CS0436 (NoWarn) kontrol edildi.
• Smoke varsa koşuldu; output/ altındaki UBL dosyaları listelendi.

Manuel doğrulama önerisi:
1) output/ altındaki örnek UBL'yi açın; şu alanları gözle: 
   cbc:InvoiceTypeCode, cbc:IssueDate, cac:TaxTotal/cbc:TaxAmount, cac:LegalMonetaryTotal/*
2) UZ: VKN 9 hane, currency UZS; KZ: BIN 12 hane, currency KZT; TR: TRY.
3) Eksik Supports/SupportsCountry raporu varsa ilgili provider'a minimal gövde ekleyin.
EOF

echo
echo "===== ✅ Analiz tamam ====="
