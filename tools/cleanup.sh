#!/bin/bash
# Cleanup script for sandbox testing

set -Eeuo pipefail

echo "== SANDBOX CLEANUP başlıyor =="

# 1) Eski SIMULASYON dosyalarını archive'e taşı
if [[ -d "output" ]]; then
    echo "• Eski SIMULASYON dosyaları arşivleniyor..."
    mkdir -p archive/old-output
    
    find output -maxdepth 1 -type f -name "SIMULASYON_*" -size -1M -print0 2>/dev/null | while IFS= read -r -d '' f; do
        echo "  📁 Arşivleniyor: $f"
        mv "$f" archive/old-output/ || true
    done
    
    # Eski SANDBOX dosyalarını da arşivle
    find output -maxdepth 1 -type f -name "*_SANDBOX_*" -size -1M -print0 2>/dev/null | while IFS= read -r -d '' f; do
        echo "  📁 Arşivleniyor: $f"
        mv "$f" archive/old-output/ || true
    done
fi

# 2) Log dosyalarını döndür (7 günden eski)
if [[ -d "logs" ]]; then
    echo "• Eski log dosyaları temizleniyor..."
    
    # 7 günden eski log dosyalarını bul ve sil
    find logs -type f -name "*.log" -mtime +7 -print0 2>/dev/null | while IFS= read -r -d '' f; do
        echo "  🗑️ Eski log siliniyor: $f"
        rm -f "$f" || true
    done
    
    # Boş log dosyalarını temizle
    find logs -type f -name "*.log" -size 0 -print0 2>/dev/null | while IFS= read -r -d '' f; do
        echo "  🗑️ Boş log siliniyor: $f"
        rm -f "$f" || true
    done
fi

# 3) Response dosyalarını temizle (güvenlik için)
if [[ -d "output/responses" ]]; then
    echo "• Response dosyaları temizleniyor (güvenlik)..."
    
    # Sadece .gitkeep dosyasını tut
    find output/responses -type f ! -name ".gitkeep" -print0 2>/dev/null | while IFS= read -r -d '' f; do
        echo "  🗑️ Response siliniyor: $f"
        rm -f "$f" || true
    done
fi

# 4) Build cache temizliği
echo "• Build cache temizleniyor..."
find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + 2>/dev/null || true

# 5) Geçici dosyalar
echo "• Geçici dosyalar temizleniyor..."
find . -type f \( -name "*.tmp" -o -name "*.temp" -o -name "*.cache" \) -size -1M -print0 2>/dev/null | while IFS= read -r -d '' f; do
    echo "  🗑️ Geçici dosya siliniyor: $f"
    rm -f "$f" || true
done

echo "== SANDBOX CLEANUP tamamlandı =="
echo "📁 Archive: archive/old-output/"
echo "📁 Logs: logs/ (7 günden eski temizlendi)"
echo "📁 Responses: output/responses/ (güvenlik için temizlendi)"
echo "🗑️ Build cache temizlendi"
