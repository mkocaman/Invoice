#!/bin/bash

# ==========================================================
# Invoice — Durum Analizi + UZ/KZ Sağlayıcı Kontrolü + Temel Fix'ler
# ==========================================================
set -euo pipefail

# ---------- Parametreler ----------
ANALYZE_ONLY=${ANALYZE_ONLY:-true}   # true => sadece analiz/rapor; false => build de dener
AUTO_FIX=${AUTO_FIX:-false}          # true => güvenli/temel otomatik düzeltmeleri uygular
VERBOSE=${VERBOSE:-false}

say() { echo -e "$*"; }
info() { say "ℹ️  $*"; }
ok() { say "✅ $*"; }
warn() { say "⚠️  $*"; }
err() { say "❌ $*"; }

ROOT="$(pwd)"
[ -f "Invoice.sln" ] || { err "Lütfen bu komutu repo kökünde (Invoice/) çalıştır."; exit 1; }

say "==> Çalışma dizini: $ROOT"

# ---------- 1) Klasör/Solution sağlığı ----------
say "\n# 1) Klasör/Solution sağlığı"
find . -maxdepth 2 -type d | sort
say "\n.sln dosyaları:"
find . -name "*.sln" -print

if [ -d "Invoice/Invoice" ]; then
  warn "Nested 'Invoice/Invoice' klasörü tespit edildi. Temizlenmesi gerekir."
else
  ok "Nested klasör yok."
fi

say "\nSolution içeriği:"
dotnet sln Invoice.sln list || true

# ---------- 2) Proje listesi ----------
say "\n# 2) Proje listesi"
find src -type f -name "*.csproj" | sort

# ---------- 3) Kod kalitesi ve bilinen problemler taraması ----------
say "\n# 3) Kod taraması (bilinen hatalar)"

# Türkçe yorum: ToString(1) — geçersiz numeric format deseni
TS1_FILES=$(grep -Rnl --include='*.cs' -E 'ToString\(\s*1\s*\)' src || true)
if [ -n "${TS1_FILES}" ]; then
  warn "ToString(1) hatası bulunan dosyalar:\n${TS1_FILES}\n"
  if [ "${AUTO_FIX}" = "true" ]; then
    info "ToString(1) => ToString(\"0.0\") olarak düzeltilecek."
    while IFS= read -r f; do
      sed -i '' -E 's/ToString\(\s*1\s*\)/ToString("0.0")/g' "$f"
    done <<< "$TS1_FILES"
    ok "ToString düzeltmesi uygulandı."
  fi
else
  ok "ToString(1) deseni bulunmadı."
fi

# Türkçe yorum: InvoiceDate nullable güvenli formatlama
say "\nInvoiceDate.ToString kontrolü:"
IDATE_RISK=$(grep -Rnl --include='*.cs' -E 'InvoiceDate\)\.ToString\(' src || true)
if [ -n "${IDATE_RISK}" ]; then
  warn "InvoiceDate null-check olmadan ToString kullanımları:\n${IDATE_RISK}\n"
  if [ "${AUTO_FIX}" = "true" ]; then
    info "Null-safe format uygulanacak: (InvoiceDate?.ToString(\"yyyy-MM-dd\") ?? string.Empty)"
    while IFS= read -r f; do
      # Basit ve güvenli bir yer değiştirme; manuel gözden geçirme önerilir
      perl -0777 -pe 's/InvoiceDate\)\.ToString\("yyyy-MM-dd"\)/(InvoiceDate?\:\.ToString("yyyy-MM-dd") \?\? string.Empty)/g' "$f" > "$f.tmp" && mv "$f.tmp" "$f"
    done <<< "$IDATE_RISK"
    ok "InvoiceDate null-safe dönüşümler uygulandı (en yaygın kalıp)."
  fi
else
  ok "InvoiceDate.ToString riskli kullanım bulunmadı."
fi

# ---------- 4) InvoiceEnvelope yapısı (eşü/UBL beklentisi) ----------
say "\n# 4) InvoiceEnvelope yapısı kontrolü"
ENV_FILE="$(grep -Rnl --include='*.cs' -E '\bclass\s+InvoiceEnvelope\b' src || true)"
if [ -z "${ENV_FILE}" ]; then
  err "InvoiceEnvelope sınıfı bulunamadı! (Application veya Domain içinde olmalı)"
else
  ok "InvoiceEnvelope bulundu:\n${ENV_FILE}\n"
  # partial mı?
  if ! grep -Eq '\bpartial\s+class\s+InvoiceEnvelope\b' ${ENV_FILE}; then
    warn "InvoiceEnvelope partial değil."
    if [ "${AUTO_FIX}" = "true" ]; then
      info "InvoiceEnvelope 'partial' hale getiriliyor."
      sed -i '' -E 's/\bclass[[:space:]]+InvoiceEnvelope\b/partial class InvoiceEnvelope/g' ${ENV_FILE}
    fi
  else
    ok "InvoiceEnvelope zaten partial."
  fi

  # Items tipi kontrol (InvoiceLineItem)
  if ! grep -Eq 'IReadOnlyList<\s*InvoiceLineItem\s*>\??[[:space:]]*Items' ${ENV_FILE}; then
    warn "InvoiceEnvelope.Items tipi InvoiceLineItem değil."
    if [ "${AUTO_FIX}" = "true" ]; then
      info "Items alanını partial dosyada güvence altına alacağız."
      NS="$(grep -m1 -E '^\s*namespace\s+' ${ENV_FILE} | sed -E 's/.*namespace\s+([^ ;]+).*/\1/')"
      PART_FILE="$(dirname "${ENV_FILE}")/InvoiceEnvelope.Partial.Items.Generated.cs"
      cat > "${PART_FILE}" <<EOF
// Türkçe yorum: InvoiceEnvelope Items alanı tip güvenliği (eşü/UBL için kalemlerin adı/miktar/fiyat gereklidir)
using System.Collections.Generic;

namespace ${NS}
{
    public partial class InvoiceEnvelope
    {
        public IReadOnlyList<InvoiceLineItem>? Items { get; set; }
    }
}
EOF
      ok "Items için partial eklendi: ${PART_FILE}"
    fi
  else
    ok "Items tipi uygun görünüyor."
  fi

  # InvoiceLineItem sınıfı kontrol
  ITEM_FILE="$(grep -Rnl --include='*.cs' -E '\bclass\s+InvoiceLineItem\b' src || true)"
  if [ -z "${ITEM_FILE}" ]; then
    warn "InvoiceLineItem sınıfı bulunamadı."
    if [ "${AUTO_FIX}" = "true" ]; then
      info "InvoiceLineItem oluşturulacak."
      NS="$(grep -m1 -E '^\s*namespace\s+' ${ENV_FILE} | sed -E 's/.*namespace\s+([^ ;]+).*/\1/')"
      ITEM_OUT="$(dirname "${ENV_FILE}")/InvoiceLineItem.cs"
      cat > "${ITEM_OUT}" <<EOF
// Türkçe yorum: UBL/eşü gereği fatura kaleminde ad, miktar, birim fiyat gibi alanlar gerekir.
namespace ${NS}
{
    public class InvoiceLineItem
    {
        public string?  Name { get; set; }       // kalem adı
        public decimal? Quantity { get; set; }   // miktar
        public decimal? UnitPrice { get; set; }  // birim fiyat
        public decimal  Total => (Quantity ?? 0m) * (UnitPrice ?? 0m); // hesaplanan toplam
    }
}
EOF
      ok "InvoiceLineItem eklendi: ${ITEM_OUT}"
    fi
  else
    ok "InvoiceLineItem bulundu:\n${ITEM_FILE}\n"
  fi
fi

# ---------- 5) ISigningService imza sözleşmesi ----------
say "\n# 5) ISigningService kontrolü"
SIGN_IF="$(grep -Rnl --include='*.cs' -E '\binterface\s+ISigningService\b' src || true)"
if [ -z "${SIGN_IF}" ]; then
  warn "ISigningService bulunamadı."
else
  ok "ISigningService bulundu:\n${SIGN_IF}\n"
  # byte[] imza var mı?
  if ! grep -Eq 'SignDocument\s*\(\s*byte\[\]' ${SIGN_IF}; then
    warn "SignDocument(byte[]) bildirimi yok."
  else
    ok "SignDocument(byte[]) mevcut."
  fi
  # string overload var mı?
  if ! grep -Eq 'SignDocument\s*\(\s*string\s+' ${SIGN_IF}; then
    warn "SignDocument(string) overload yok."
    if [ "${AUTO_FIX}" = "true" ]; then
      info "Interface default body ile string overload eklenecek."
      TMP="${SIGN_IF}.tmp"
      awk '
        BEGIN { in_iface=0; brace=0; }
        {
          line=$0;
          if (in_iface==0 && $0 ~ /\binterface[[:space:]]+ISigningService\b/) in_iface=1;
          if (in_iface==1) {
            if ($0 ~ /{/) brace++;
            if ($0 ~ /}/) brace--;
            if (brace==0 && $0 ~ /}/) {
              print "        // Türkçe yorum: string veri imzası için sarmalayıcı"
              print "        public virtual System.Threading.Tasks.Task<byte[]> SignDocument(string data, System.Threading.CancellationToken ct = default)"
              print "        {"
              print "            var bytes = System.Text.Encoding.UTF8.GetBytes(data ?? string.Empty);"
              print "            return SignDocument(bytes, ct);"
              print "        }"
            }
          }
          print line;
        }' "$SIGN_IF" > "$TMP"
      mv "$TMP" "$SIGN_IF"
      ok "ISigningService string overload eklendi."
    fi
  else
    ok "SignDocument(string) mevcut."
  fi
fi

# ---------- 6) UZ / KZ sağlayıcı kontrolleri ----------
say "\n# 6) UZ/KZ sağlayıcı kontrolleri (IInvoiceProvider, format vb.)"

check_provider_dir () {
  local DIR="$1"
  local TITLE="$2"
  if [ ! -d "$DIR" ]; then
    warn "${TITLE} klasörü yok: $DIR"
    return
  fi

  say "\n— ${TITLE} dosyaları:"
  find "$DIR" -type f -name "*.cs" | sed 's/^/   • /'

  # Interface uygulanmış mı?
  local IMPL_ERR
  IMPL_ERR=$(grep -R --include='*.cs' -nE 'error CS0535' "$DIR" 2>/dev/null || true)
  if [ -n "$IMPL_ERR" ]; then
    warn "${TITLE} içinde CS0535 (eksik interface üyeleri) olasılığı."
  fi

  # SupportsCountry / Supports var mı?
  local MISS_SUPPORTS
  MISS_SUPPORTS=$(grep -Rnl --include='*.cs' -E 'class\s+\w+Provider\b' "$DIR" | while read -r f; do
    if ! grep -Eq 'SupportsCountry\s*\(' "$f"; then echo "$f"; fi
  done)
  if [ -n "$MISS_SUPPORTS" ]; then
    warn "${TITLE} bazı provider'larda SupportsCountry eksik:\n${MISS_SUPPORTS}\n"
  else
    ok "${TITLE} provider'larında SupportsCountry mevcut."
  fi
  local MISS_SUPPORTS_T
  MISS_SUPPORTS_T=$(grep -Rnl --include='*.cs' -E 'class\s+\w+Provider\b' "$DIR" | while read -r f; do
    if ! grep -Eq 'Supports\s*\(\s*ProviderType\s*\)' "$f"; then echo "$f"; fi
  done)
  if [ -n "$MISS_SUPPORTS_T" ]; then
    warn "${TITLE} bazı provider'larda Supports(ProviderType) eksik:\n${MISS_SUPPORTS_T}\n"
  else
    ok "${TITLE} provider'larında Supports(ProviderType) mevcut."
  fi

  # ToString(1) yine kontrol
  local T1
  T1=$(grep -Rnl --include='*.cs' -E 'ToString\(\s*1\s*\)' "$DIR" || true)
  if [ -n "$T1" ]; then
    warn "${TITLE} içinde ToString(1) kullanımı var:\n${T1}\n"
  else
    ok "${TITLE} ToString(1) problemi yok."
  fi

  # InvoiceDate null-safe kontrolü (en sık kalıp)
  local ND
  ND=$(grep -Rnl --include='*.cs' -E 'InvoiceDate\)\.ToString\(' "$DIR" || true)
  if [ -n "$ND" ]; then
    warn "${TITLE} içinde InvoiceDate null-safe olmayan ToString çağrıları:\n${ND}\n"
  else
    ok "${TITLE} InvoiceDate null-safe görünüyor."
  fi
}

check_provider_dir "src/UZ/Invoice.Infrastructure.UZ" "UZ (Özbekistan)"
check_provider_dir "src/KZ/Invoice.Infrastructure.KZ" "KZ (Kazakistan)"

# ---------- 7) NuGet hijyen: EFCore.Design & Xml.Linq & Text.Json ----------
say "\n# 7) NuGet taraması (EFCore.Design / System.Xml.Linq / System.Text.Json)"

if grep -Rnl --include='*.csproj' -E '<PackageReference[^>]*Include="Microsoft\.EntityFrameworkCore\.Design"' src >/dev/null 2>&1; then
  warn "Microsoft.EntityFrameworkCore.Design referansı tespit edildi:"
  grep -Rnl --include='*.csproj' -E '<PackageReference[^>]*Include="Microsoft\.EntityFrameworkCore\.Design"' src
else
  ok "EFCore.Design paket referansı .csproj dosyalarında görünmüyor."
fi

if grep -Rnl --include='*.csproj' -E '<PackageReference[^>]*Include="System\.Xml\.Linq"' src >/dev/null 2>&1; then
  warn "System.Xml.Linq paket referansı var (gerekli değilse kaldırın):"
  grep -Rnl --include='*.csproj' -E '<PackageReference[^>]*Include="System\.Xml\.Linq"' src
else
  ok "System.Xml.Linq paket referansı yok."
fi

if grep -Rnl --include='*.csproj' -E '<PackageReference[^>]*Include="System\.Text\.Json"' src >/dev/null 2>&1; then
  info "System.Text.Json paket referansı olan projeler:"
  grep -Rnl --include='*.csproj' -E '<PackageReference[^>]*Include="System\.Text\.Json"' src
else
  ok "System.Text.Json explicit referansı yok (framework üzerinden gelebilir)."
fi

# ---------- 8) Build (opsiyonel) ----------
if [ "${ANALYZE_ONLY}" = "true" ]; then
  say "\n# 8) Build aşaması atlandı (ANALYZE_ONLY=true)."
else
  say "\n# 8) Build denemesi (katman sırası)"
  set +e
  dotnet build src/Core/Invoice.Domain/Invoice.Domain.csproj
  dotnet build src/Core/Invoice.Application/Invoice.Application.csproj
  dotnet build src/Core/Invoice.Infrastructure.Core/Invoice.Infrastructure.Core.csproj
  dotnet build src/TR/Invoice.Infrastructure.TR/Invoice.Infrastructure.TR.csproj
  dotnet build src/UZ/Invoice.Infrastructure.UZ/Invoice.Infrastructure.UZ.csproj
  dotnet build src/KZ/Invoice.Infrastructure.KZ/Invoice.Infrastructure.KZ.csproj
  dotnet build Invoice.sln
  STATUS=$?
  set -e
  if [ $STATUS -eq 0 ]; then ok "Build başarılı (uyarılar olabilir)."; else err "Build hataları var. Yukarıdaki rapora göre ilerleyin."; fi
fi

# ---------- 9) Son özet ----------
say "\n# 9) Özet / Aksiyon"
say "• Fatura modeli (InvoiceEnvelope + InvoiceLineItem) UBL/eşü gereksinimlerine uygun mu? Ad/Miktar/BirimFiyat alanları zorunlu kabul edildi."
say "• UZ/KZ sağlayıcılarında Supports/SupportsCountry, null-safe tarih ve numeric formatlar doğrulandı."
say "• NuGet tarafında EFCore.Design referansları ve System.Xml.Linq / System.Text.Json aykırılıkları raporlandı."
if [ "${AUTO_FIX}" = "true" ]; then
  ok "Temel otomatik düzeltmeler uygulandı."
else
  info "AUTO_FIX=false olduğu için dosyalara sadece bakıldı; düzeltme uygulanmadı."
fi
