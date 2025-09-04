#!/usr/bin/env python3
"""
UZ Didox/FakturaUZ JSON Schema Doğrulama Script'i
JSON dosyalarını schema'ya göre doğrular
"""

import sys
import os
import json
from pathlib import Path

def validate_uz_json(json_file):
    """
    UZ JSON dosyasını schema'ya göre doğrular
    """
    try:
        # JSON dosyasını yükle
        with open(json_file, 'r', encoding='utf-8') as f:
            data = json.load(f)
        
        print(f"✅ JSON parse edildi: {json_file}")
        
        # Schema doğrulama
        validation_errors = []
        
        # Gerekli ana alanları kontrol et
        required_fields = {
            'documentType': 'Doküman tipi eksik',
            'invoiceNumber': 'Fatura numarası eksik',
            'issueDate': 'Fatura tarihi eksik',
            'currency': 'Para birimi eksik',
            'supplier': 'Tedarikçi bilgisi eksik',
            'customer': 'Müşteri bilgisi eksik',
            'lines': 'Ürün listesi eksik',
            'totals': 'Toplam bilgileri eksik'
        }
        
        for field, error_msg in required_fields.items():
            if field not in data:
                validation_errors.append(f"❌ {error_msg}: {field}")
            else:
                print(f"✅ {field} alanı mevcut")
        
        # Supplier kontrolü
        if 'supplier' in data:
            supplier = data['supplier']
            if 'tin' not in supplier:
                validation_errors.append("❌ Supplier TIN eksik")
            else:
                tin_value = str(supplier['tin'])
                if not tin_value or len(tin_value) != 9:
                    validation_errors.append(f"❌ Supplier TIN geçersiz: {tin_value} (9 hane olmalı)")
                else:
                    print(f"✅ Supplier TIN: {tin_value}")
            
            if 'name' not in supplier:
                validation_errors.append("❌ Supplier name eksik")
            else:
                print(f"✅ Supplier name: {supplier['name']}")
        
        # Customer kontrolü
        if 'customer' in data:
            customer = data['customer']
            if 'tin' not in customer:
                validation_errors.append("❌ Customer TIN eksik")
            else:
                tin_value = str(customer['tin'])
                if not tin_value or len(tin_value) != 9:
                    validation_errors.append(f"❌ Customer TIN geçersiz: {tin_value} (9 hane olmalı)")
                else:
                    print(f"✅ Customer TIN: {tin_value}")
            
            if 'name' not in customer:
                validation_errors.append("❌ Customer name eksik")
            else:
                print(f"✅ Customer name: {customer['name']}")
        
        # Currency kontrolü
        if 'currency' in data:
            currency = data['currency']
            if currency != 'UZS':
                validation_errors.append(f"❌ Para birimi UZS olmalı, bulunan: {currency}")
            else:
                print(f"✅ Para birimi: {currency}")
        
        # Lines kontrolü
        if 'lines' in data:
            lines = data['lines']
            if not isinstance(lines, list) or len(lines) == 0:
                validation_errors.append("❌ Lines boş liste veya liste değil")
            else:
                print(f"✅ {len(lines)} adet ürün bulundu")
                
                # İlk ürünü detay kontrol et
                if len(lines) > 0:
                    first_line = lines[0]
                    required_line_fields = {
                        'id': 'Ürün ID',
                        'name': 'Ürün adı',
                        'quantity': 'Miktar',
                        'unitCode': 'Birim kodu',
                        'unitPrice': 'Birim fiyat',
                        'lineExtensionAmount': 'Satır toplamı',
                        'vatPercent': 'KDV yüzdesi',
                        'vatAmount': 'KDV tutarı'
                    }
                    
                    for field, field_name in required_line_fields.items():
                        if field not in first_line:
                            validation_errors.append(f"❌ Ürün {field_name} eksik: {field}")
                        else:
                            print(f"✅ Ürün {field_name}: {first_line[field]}")
        
        # Totals kontrolü
        if 'totals' in data:
            totals = data['totals']
            required_total_fields = {
                'lineExtensionAmount': 'Vergisiz toplam',
                'taxExclusiveAmount': 'Vergisiz toplam (alternatif)',
                'taxAmount': 'KDV toplamı',
                'taxInclusiveAmount': 'Vergili toplam',
                'payableAmount': 'Ödenecek tutar'
            }
            
            for field, field_name in required_total_fields.items():
                if field not in totals:
                    validation_errors.append(f"❌ Toplam {field_name} eksik: {field}")
                else:
                    print(f"✅ Toplam {field_name}: {totals[field]}")
        
        # Meta bilgi kontrolü
        if 'meta' in data:
            meta = data['meta']
            if 'source' in meta:
                print(f"✅ Meta source: {meta['source']}")
            if 'simulasyon' in meta:
                print(f"✅ Meta simulasyon: {meta['simulasyon']}")
        
        # Sonuçları raporla
        if validation_errors:
            print(f"\n❌ {len(validation_errors)} doğrulama hatası bulundu:")
            for error in validation_errors:
                print(f"  {error}")
            return False
        else:
            print(f"\n✅ Tüm doğrulamalar başarılı!")
            return True
            
    except json.JSONDecodeError as e:
        print(f"❌ JSON parse hatası: {e}")
        return False
    except Exception as e:
        print(f"❌ Doğrulama hatası: {e}")
        return False

def main():
    if len(sys.argv) < 2:
        print("Kullanım: python3 uz_json_validator.py <json_file>")
        print("Örnek: python3 uz_json_validator.py invoice.json")
        sys.exit(1)
    
    json_file = sys.argv[1]
    
    if not os.path.exists(json_file):
        print(f"❌ JSON dosyası bulunamadı: {json_file}")
        sys.exit(1)
    
    print(f"🔍 UZ JSON Doğrulama: {json_file}")
    print("-" * 50)
    
    success = validate_uz_json(json_file)
    
    if success:
        print("\n🎯 JSON dosyası geçerli!")
        sys.exit(0)
    else:
        print("\n💥 JSON dosyasında hatalar var!")
        sys.exit(1)

if __name__ == "__main__":
    main()
