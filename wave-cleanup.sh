set -Eeuo pipefail
export RPROMPT="" || true

echo "== WAVE-CLEANUP (KZ/UZ) başlıyor =="

###############################################################################
# 1) Araştırma sürecinde indirilen, artık kullanılmayan repoları temizle
###############################################################################
if [[ -d repos ]]; then
  echo "• repos/ klasörü siliniyor (public repo klonları)"
  rm -rf repos
fi

###############################################################################
# 2) Geçici / araştırma dosyalarını temizle
###############################################################################
rm -rf research/tmp || true
rm -rf input/*.zip || true
rm -rf input/*.json || true
rm -rf tools/uz_normalize.py tools/kz_native_from_norm.py || true

###############################################################################
# 3) Eski / tekrar üretilebilen raporları arşivle veya kaldır
###############################################################################
mkdir -p archive/old-reports
for f in docs/reports/*.md; do
  case "$f" in
    *STATUS_RESEARCH_VALIDATE.md|*RESEARCH_KZ_UZ_SUMMARY.md) 
      echo "• Rapor tutuluyor: $f"
      ;;
    *) 
      echo "• Arşivleniyor: $f"
      mv "$f" archive/old-reports/ || true
      ;;
  esac
done

###############################################################################
# 4) Output klasörünü sadeleştir
###############################################################################
mkdir -p archive/old-output
find output -maxdepth 1 -type f -name "SIMULASYON_*" -size -1M -print0 | while IFS= read -r -d '' f; do
  echo "• Arşivleniyor: $f"
  mv "$f" archive/old-output/ || true
done

###############################################################################
# 5) Build cache temizliği
###############################################################################
find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + || true
rm -rf ~/.nuget/packages || true
rm -rf ~/.nuget/http-cache || true

echo "== WAVE-CLEANUP tamamlandı =="
echo "Kalan yapılar: docs/, tools/, output/normalized/, appsettings.*.json"
