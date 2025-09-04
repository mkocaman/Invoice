#!/usr/bin/env bash
set -euo pipefail
echo "— countries —"; for d in src/TR src/UZ src/KZ; do [ -d "$d" ] && echo "✓ $d" || echo "✗ $d EKSİK"; done
echo "— providers —"; [ -d src/Infrastructure/Providers/Adapters ] && grep -R "class .*Adapter" src/Infrastructure/Providers/Adapters | wc -l
echo "— build —"; dotnet build ./Invoice.sln -v minimal
echo "OK"
