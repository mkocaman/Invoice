#!/usr/bin/env bash
# ============================================================
 # Invoice — NuGet/Derleme Stabilizasyonu + Platform Uyumlu Bakım
# macOS/Linux/Windows uyumlu, soru sormaz; gerekeni yapar.
# ============================================================
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

root_hint="Invoice.sln"
if [ ! -f "$root_hint" ]; then
  echo "❌ Lütfen script'i repo KÖKÜNDE (Invoice/) çalıştırın. Bulunamadı: $root_hint"
  exit 1
fi

echo "==> Çalışma dizini: $(pwd)"
echo

# -----------------------------
# 0) Yardımcı fonksiyonlar
# -----------------------------
# Türkçe: Bir .csproj içinden belirlenen PackageReference bloklarını (tek satır veya çok satır) kaldır.
remove_pkg_block () {
  # $1: csproj yolu, $2..: paket adları (regex güvenli)
  local csproj="$1"; shift
  [ -f "$csproj" ] || return 0
  # Perl ile tüm dosyayı içeri alıp PackageReference bloklarını güvenli biçimde temizliyoruz.
  local pattern=""
  for pkg in "$@"; do
    local escaped=$(printf '%s\n' "$pkg" | $SED_CMD 's/[.[\*^$(){}+?|]/\\&/g')
    pattern="${pattern}${pattern:+|}${escaped}"
  done
  perl -0777 -i -pe "s#\\s*<PackageReference[^>]*Include=\\\"(${pattern})\\\"[^>]*/>\\s*\\n##gis" "$csproj"
  perl -0777 -i -pe "s#\\s*<PackageReference[^>]*Include=\\\"(${pattern})\\\"[^>]*>.*?</PackageReference>\\s*##gis" "$csproj"
}

# Türkçe: Bir .csproj içine, aynı Include yoksa PackageReference ekle
append_pkg_ref () {
  # $1: csproj, $2: Include, $3: opsiyonel: iç XML (örn PrivateAssets)
  local csproj="$1" inc="$2" inner="${3:-}"
  [ -f "$csproj" ] || return 0
  if ! grep -q "PackageReference[^>]*Include=\"${inc}\"" "$csproj"; then
    # Son </Project> öncesine ItemGroup ile ekle (var olan ItemGrouplara dokunmadan)
    if grep -q "</ItemGroup>" "$csproj"; then
      # Son ItemGroup kapanışından önce ekle
      perl -0777 -i -pe "s#</ItemGroup>#  <PackageReference Include=\"${inc}\">${inner:+\n    ${inner}\n  }</PackageReference>\n</ItemGroup>#s" "$csproj"
    else
      # ItemGroup yoksa oluştur
      perl -0777 -i -pe "s#</Project>#  <ItemGroup>\n    <PackageReference Include=\"${inc}\">${inner:+\n      ${inner}\n    }</PackageReference>\n  </ItemGroup>\n</Project>#s" "$csproj"
    fi
  fi
}

# -----------------------------
# 1) Merkezi paket versiyonları
# -----------------------------
# Türkçe: System.Text.Json / System.Xml.Linq'i NuGet'tan yönetME. .NET 9'da framework ile gelir.
cat > Directory.Packages.props <<'XML'
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- Yalnız EF paket sürümleri (merkezi) -->
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
  </ItemGroup>
</Project>
XML
echo "✅ Directory.Packages.props yazıldı."

# -----------------------------
# 2) Tüm .csproj’lardan Json/Xml.Linq PaketReferanslarını kaldır
# -----------------------------
while IFS= read -r -d '' P; do
  remove_pkg_block "$P" "System.Text.Json" "System.Xml.Linq"
done < <(find src -name '*.csproj' -print0)
echo "✅ Tüm csproj'larda System.Text.Json / System.Xml.Linq PaketReferans satırları kaldırıldı."

# -----------------------------
# 3) Domain ve Application’dan EF paketlerini sök
# -----------------------------
DOM="src/Core/Invoice.Domain/Invoice.Domain.csproj"
APP="src/Core/Invoice.Application/Invoice.Application.csproj"

for PROJ in "$DOM" "$APP"; do
  if [ -f "$PROJ" ]; then
    remove_pkg_block "$PROJ" \
      "Microsoft.EntityFrameworkCore" \
      "Microsoft.EntityFrameworkCore.Design" \
      "Npgsql.EntityFrameworkCore.PostgreSQL"
  fi
done
echo "✅ Domain/Application katmanlarından EF paketleri temizlendi."

# -----------------------------
# 4) Infrastructure.Core’a doğru EF paketlerini kur
# -----------------------------
INF="src/Core/Invoice.Infrastructure.Core/Invoice.Infrastructure.Core.csproj"
if [ -f "$INF" ]; then
  # Eski kopyaları temizle
  remove_pkg_block "$INF" \
    "Microsoft.EntityFrameworkCore" \
    "Microsoft.EntityFrameworkCore.Design" \
    "Npgsql.EntityFrameworkCore.PostgreSQL" \
    "System.Text.Json" \
    "System.Xml.Linq"
  # Gerekli EF referanslarını tekrar ekle (versiyonsuz; merkezden gelir)
  append_pkg_ref "$INF" "Microsoft.EntityFrameworkCore"
  append_pkg_ref "$INF" "Npgsql.EntityFrameworkCore.PostgreSQL"
  # Design YALNIZ buraya, PrivateAssets=all ile (transitif taşınmasın)
  if ! grep -q 'Microsoft.EntityFrameworkCore.Design' "$INF"; then
    append_pkg_ref "$INF" "Microsoft.EntityFrameworkCore.Design" "<PrivateAssets>all</PrivateAssets>\n    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"
  fi
  echo "✅ Infrastructure.Core EF referansları düzenlendi (Design = PrivateAssets:all)."
else
  echo "⚠️  Infrastructure.Core csproj bulunamadı: $INF"
fi

# -----------------------------
# 5) KZ: System.Web kalıntılarını temizle (WebUtility'e taşı)
# -----------------------------
KZ_DIR="src/KZ/Invoice.Infrastructure.KZ"
if [ -d "$KZ_DIR" ]; then
  while IFS= read -r -d '' f; do
    # Türkçe: using düzeltmeleri
    perl -i -pe 's#^(\s*)using\s+System\.Web\s*;\s*$#$1using System.Net;\n#' "$f"
    perl -i -pe 's#^(\s*)using\s+System\.Web\.WebUtility\s*;\s*$#$1using System.Net;\n#' "$f"
    # Türkçe: sınıf adı ve çağrı düzeltmeleri
    perl -i -pe 's#\bSystem\.Web\.WebUtility\b#System.Net.WebUtility#g' "$f"
    perl -i -pe 's#\bHttpUtility\.HtmlEncode\b#WebUtility.HtmlEncode#g' "$f"
    perl -i -pe 's#\bHttpUtility\.HtmlDecode\b#WebUtility.HtmlDecode#g' "$f"
  done < <(find "$KZ_DIR" -name '*.cs' -print0)
  echo "✅ KZ: System.Web → System.Net.WebUtility migrasyonu garanti edildi."
fi

# -----------------------------
# 6) Gereksiz Class1.cs dosyalarını sil
# -----------------------------
for junk in \
  "src/UZ/Invoice.Infrastructure.UZ/Class1.cs" \
  "src/KZ/Invoice.Infrastructure.KZ/Class1.cs"
do
  [ -f "$junk" ] && { rm -f "$junk"; echo "🧹 Silindi: $junk"; }
done

# -----------------------------
# 7) Eski build artıkları + NuGet cache temizliği
# -----------------------------
find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + || true
find . -name "packages.lock.json" -delete || true
dotnet nuget locals all --clear || true
echo "🧹 Temizlik tamam."

# -----------------------------
# 8) Restore + sıralı build (hata pinleme)
# -----------------------------
set +e
dotnet restore --force --no-cache
R=$?

dotnet build src/Core/Invoice.Domain/Invoice.Domain.csproj -v minimal
BD=$?
dotnet build src/Core/Invoice.Application/Invoice.Application.csproj -v minimal
BA=$?
dotnet build src/Core/Invoice.Infrastructure.Core/Invoice.Infrastructure.Core.csproj -v minimal
BI=$?
dotnet build src/TR/Invoice.Infrastructure.TR/Invoice.Infrastructure.TR.csproj -v minimal
BTR=$?
dotnet build src/UZ/Invoice.Infrastructure.UZ/Invoice.Infrastructure.UZ.csproj -v minimal
BUZ=$?
dotnet build src/KZ/Invoice.Infrastructure.KZ/Invoice.Infrastructure.KZ.csproj -v minimal
BKZ=$?
dotnet build Invoice.sln -v minimal
BSLN=$?
set -e

echo
echo "================ Derleme Sonuç Özeti (0=başarı) ================"
echo "Restore..............: $R"
echo "Domain...............: $BD"
echo "Application..........: $BA"
echo "Infra.Core...........: $BI"
echo "TR...................: $BTR"
echo "UZ...................: $BUZ"
echo "KZ...................: $BKZ"
echo "Solution (toplam)....: $BSLN"
echo "==============================================================="

if [ $R -ne 0 ] || [ $BSLN -ne 0 ]; then
  echo "❌ Build/restore hataları kaldı. Çıktıyı paylaş; kalan hataya göre ilgili .csproj/dosyaların MEVCUT HALİNİ isteyip tam kod düzeltmesi göndereceğim."
else
  echo "✅ Tüm derlemeler BAŞARILI."
fi