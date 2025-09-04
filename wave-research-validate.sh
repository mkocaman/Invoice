set -Eeuo pipefail
export RPROMPT="" || true

echo "== WAVE-RESEARCH→VALIDATE (KZ/UZ) v2 =="
mkdir -p docs/samples docs/findings docs/reports output tools research/tmp

###############################################################################
# 1) ÖN OKUMA: Mevcut araştırma dosyalarını toparla ve kısa özet üret
###############################################################################
: > docs/reports/RESEARCH_KZ_UZ_SUMMARY.md
{
  echo "# KZ/UZ Araştırma Özeti — [SIMULASYON]"
  echo ""
  echo "## Kaynak Notlar"
  for f in docs/RESEARCH_KZ_UZ.md docs/TODO_KZ_UZ.md docs/API-MAPPING_KZ.md docs/API-MAPPING_UZ.md docs/STATUS_UZ_SAMPLES.md docs/STATUS_UZ_KZ.md docs/STATUS_KZ_SAMPLES.md docs/STATUS_KZ_SANDBOX.md docs/STATUS_UZ_SANDBOX.md; do
    if [[ -s "$f" ]]; then
      echo "### $(basename "$f")"
      sed -n '1,80p' "$f"
      echo ""
      echo "---"
      echo ""
    fi
  done
} >> docs/reports/RESEARCH_KZ_UZ_SUMMARY.md

echo "• Özet: docs/reports/RESEARCH_KZ_UZ_SUMMARY.md"

###############################################################################
# 2) ÖRNEK TOPLAMA & NORMALİZASYON (KZ native XML, UZ native JSON) — [SIMULASYON]
###############################################################################
# Mevcut samples klasörlerini tarayıp normalize edeceğiz.
mkdir -p docs/normalized

# UZ JSON normalize helper (toleranslı alan isimleri, yuvarlama ROUND_HALF_UP)
cat > tools/uz_normalize.py << 'PY'
import os,sys,json,datetime
from decimal import Decimal,ROUND_HALF_UP
def D(x): 
    try: return Decimal(str(x))
    except: return Decimal("0")
def s(x): return (x or "").strip() if isinstance(x,str) else ""
def parse_date(x):
    t=s(x)
    for fmt in ("%d.%m.%Y","%Y-%m-%d","%d/%m/%Y"):
        try: return datetime.datetime.strptime(t,fmt).strftime("%Y-%m-%d")
        except: pass
    return datetime.date.today().strftime("%Y-%m-%d")

src=sys.argv[1]; out=sys.argv[2]
data=json.loads(open(src,"rb").read().decode("utf-8","ignore"))

seller=data.get("seller") or {}
buyer =data.get("buyer")  or {}
sellertin= data.get("sellertin") or seller.get("tin") or seller.get("inn") or ""
buyertin = data.get("buyertin")  or buyer.get("tin")  or buyer.get("inn")  or ""
currency = data.get("currency") or "UZS"
docno = s(data.get("docno") or data.get("number") or "SIM-UZ-0001")
docdate= parse_date(data.get("docdate") or data.get("date"))

# ürün listesi varyantları
prods=(data.get("productlist") or {}).get("products") \
   or data.get("items") or data.get("lines") or []

lines=[]; net=Decimal("0"); tax=Decimal("0")
for i,p in enumerate(prods, start=1):
    name=s(p.get("name") or p.get("title"))
    qty=D(p.get("quantity") or p.get("qty") or "1")
    unit=s(p.get("unitcode") or p.get("unit_code") or p.get("unit") or "C62")
    price=D(p.get("unitprice") or p.get("price") or "0")
    line=(qty*price).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
    vatp=D(p.get("vatpercent") or p.get("vat") or p.get("nds") or "0")
    vata=(line*vatp/Decimal("100")).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
    net+=line; tax+=vata
    lines.append({
        "id": i, "name": name,
        "quantity": str(qty), "unitCode": unit,
        "unitPrice": str(price), "lineExtensionAmount": str(line),
        "vatPercent": str(vatp), "vatAmount": str(vata)
    })

gross=(net+tax).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
normalized={
  "documentType":"UZ-LOCAL",  # [SIMULASYON]
  "invoiceNumber": docno,
  "issueDate": docdate,
  "currency": currency,
  "supplier": {"tin": sellertin, "name": s(seller.get("name"))},
  "customer": {"tin": buyertin, "name": s(buyer.get("name"))},
  "lines": lines,
  "totals": {
    "lineExtensionAmount": str(net),
    "taxExclusiveAmount": str(net),
    "taxAmount": str(tax),
    "taxInclusiveAmount": str(gross),
    "payableAmount": str(gross)
  },
  "meta":{"source": os.path.basename(src), "simulasyon": True}
}
open(out,"w",encoding="utf-8").write(json.dumps(normalized,ensure_ascii=False,indent=2))
print(out)
PY

# KZ XML normalize helper (çok basit yerel iskelet) — gerçek ESF XSD değil!
cat > tools/kz_native_from_norm.py << 'PY'
import sys,json
from xml.etree.ElementTree import Element, SubElement, tostring
src=sys.argv[1]; out=sys.argv[2]
data=json.load(open(src,'r',encoding='utf-8'))
root=Element('Invoice'); SubElement(root,'Currency').text=data.get('currency','KZT')
sup=SubElement(root,'Supplier'); SubElement(sup,'BIN').text=(data.get('supplier',{}).get('tin') or '')
cus=SubElement(root,'Customer'); SubElement(cus,'TaxNumber').text=(data.get('customer',{}).get('tin') or '')
items=SubElement(root,'Items')
for ln in data.get('lines',[]):
    it=SubElement(items,'Item')
    SubElement(it,'Name').text=ln.get('name')
    SubElement(it,'Quantity').text=str(ln.get('quantity'))
    SubElement(it,'UnitCode').text=ln.get('unitCode','C62')
    SubElement(it,'UnitPrice').text=ln.get('unitPrice')
    SubElement(it,'LineExtensionAmount').text=ln.get('lineExtensionAmount')
tot=SubElement(root,'Totals')
t=data.get('totals',{})
for k in ['lineExtensionAmount','taxAmount','taxInclusiveAmount','payableAmount']:
    SubElement(tot, k[0].upper()+k[1:]).text=t.get(k)
open(out,'wb').write(tostring(root, encoding='utf-8'))
print(out)
PY

# UZ JSON örneklerini bul (senin eklediklerin dahil) ve normalize et
: > docs/reports/NORMALIZE_LOG.md
count=0
for j in $(find docs/samples repos input -type f -name "*.json" 2>/dev/null || true); do
  base="$(basename "$j")"
  out="docs/normalized/SIMULASYON_UZ_NATIVE_${base%.json}.json"
  p="$(python3 tools/uz_normalize.py "$j" "$out" || true)"
  if [[ -f "$out" ]]; then
    echo "UZ OK: $base -> $(basename "$out")" | tee -a docs/reports/NORMALIZE_LOG.md
    ((count+=1))
  else
    echo "UZ FAIL: $base" | tee -a docs/reports/NORMALIZE_LOG.md
  fi
done

echo "Toplam normalize edilen UZ JSON: $count"

# Her normalize UZ JSON'dan basit KZ-native XML türet (karşılaştırma için) — [SIMULASYON]
kzcount=0
for nj in $(find docs/normalized -type f -name "SIMULASYON_UZ_NATIVE_*.json" 2>/dev/null || true); do
  out="docs/normalized/SIMULASYON_KZ_NATIVE_${RANDOM}.xml"
  p="$(python3 tools/kz_native_from_norm.py "$nj" "$out" || true)"
  if [[ -f "$out" ]]; then
    echo "KZ(native) from UZ(norm) OK: $(basename "$nj") -> $(basename "$out")" | tee -a docs/reports/NORMALIZE_LOG.md
    ((kzcount+=1))
  fi
done

###############################################################################
# 3) KZ ESF XSD/WSDL VARSA şema doğrulaması (xmllint) — tamamı [SIMULASYON]
###############################################################################
# Not: projede gerçek ESF XSD/WSDL yoksa bu kısım sadece raporlar.
has_xsd=0
if compgen -G "docs/samples/**/**/*.xsd" > /dev/null || compgen -G "docs/samples/**/*.xsd" > /dev/null; then
  has_xsd=1
fi

: > docs/reports/VALIDATION_KZ.md
echo "# KZ ESF Şema Doğrulama — [SIMULASYON]" >> docs/reports/VALIDATION_KZ.md
if [[ "$has_xsd" -eq 1 ]]; then
  echo "Bulunan XSD dosyaları ile xmllint denemeleri yapılacak." >> docs/reports/VALIDATION_KZ.md
  XSD="$(find docs/samples -type f -name '*.xsd' | head -n1)"
  XMLS="$(find docs/normalized -type f -name 'SIMULASYON_KZ_NATIVE_*.xml' | head -n 5)"
  if command -v xmllint >/dev/null 2>&1; then
    for x in $XMLS; do
      if xmllint --noout --schema "$XSD" "$x" 2>> docs/reports/VALIDATION_KZ.md; then
        echo "- ✅ $(basename "$x") şemadan geçti ($XSD)" >> docs/reports/VALIDATION_KZ.md
      else
        echo "- ⚠️ $(basename "$x") şema uyumsuz veya XSD gerçek ESF şeması değil" >> docs/reports/VALIDATION_KZ.md
      fi
    done
  else
    echo "> xmllint bulunamadı, manuel doğrulama gerekli" >> docs/reports/VALIDATION_KZ.md
  fi
else
  echo "> XSD bulunamadı. ESF sandbox XSD/WSDL linkleri eklenmeli (TODO)." >> docs/reports/VALIDATION_KZ.md
fi

###############################################################################
# 4) HARİTA GÜNCELLEME: Mapping tablolarını örneklerden iyileştir — [SIMULASYON]
###############################################################################
# Basit sezgisel alan listesi çıkar ve mapping taslaklarına "GÖZDEN GEÇİR" notu ekle
python3 -c "
import os, json, glob
out='docs/reports/FIELD_INVENTORY.md'
os.makedirs(os.path.dirname(out), exist_ok=True)
with open(out,'w',encoding='utf-8') as f:
    f.write('# Alan Envanteri (UZ normalize) — [SIMULASYON]\n\n')
    for p in glob.glob('docs/normalized/SIMULASYON_UZ_NATIVE_*.json'):
        try:
            d=json.load(open(p,'r',encoding='utf-8'))
            fields=sorted(set(d.keys()) | {'supplier.'+k for k in (d.get('supplier') or {}).keys()} | {'customer.'+k for k in (d.get('customer') or {}).keys()})
            f.write(f'## {os.path.basename(p)}\n- ' + '\n- '.join(fields) + '\n\n')
        except Exception as e:
            f.write(f'## {os.path.basename(p)}\n- ⚠️ okunamadı: {e}\n\n')
print(out)
"

# Mapping dosyalarına "gözden geçir" damgası
for m in docs/API-MAPPING_KZ.md docs/API-MAPPING_UZ.md; do
  if [[ -f "$m" ]]; then
    echo -e "\n> Not: Bu sayfa [SIMULASYON] amaçlıdır; gerçek sandbox dökümanıyla **GÖZDEN GEÇİR**.\n" >> "$m"
  fi
done

###############################################################################
# 5) RAPOR: Genel durum ve TODO güncelle
###############################################################################
: > docs/reports/STATUS_RESEARCH_VALIDATE.md
{
  echo "# STATUS — Research→Validate (KZ/UZ) — [SIMULASYON]"
  echo ""
  echo "## Özet"
  echo "- UZ JSON örnekleri normalize edildi → docs/normalized/"
  echo "- KZ native XML örnekleri türetildi (karşılaştırma amaçlı)"
  echo "- KZ XSD/WSDL bulunamadıysa TODO'ya işlendi; varsa xmllint ile denendi"
  echo ""
  echo "## Dosyalar"
  ls -1 docs/reports | sed 's/^/- /'
  echo ""
  echo "## TODO (otomatik eklendi)"
  echo "- [ ] ESF sandbox **resmi** XSD/WSDL linklerini ekle (docs/API-MAPPING_KZ.md)"
  echo "- [ ] UZ tarafında E-IMZO token akışını gerçek endpointlerle doğrula"
  echo "- [ ] Hata kodları / retry stratejileri"
  echo "- [ ] Sertifika/keystore yapılandırması (JKS/PFX) — [SIMULASYON]"
} >> docs/reports/STATUS_RESEARCH_VALIDATE.md

echo "== DONE =="
echo "Aç: docs/reports/RESEARCH_KZ_UZ_SUMMARY.md"
echo "Aç: docs/reports/STATUS_RESEARCH_VALIDATE.md"
echo "Aç: docs/API-MAPPING_KZ.md, docs/API-MAPPING_UZ.md"
