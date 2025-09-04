#!/usr/bin/env python3
"""
KZ ESF XML Şema Doğrulama Script'i
XSD şeması ile XML dosyalarını doğrular
"""

import sys
import os
import xml.etree.ElementTree as ET
from pathlib import Path

def validate_xml_against_xsd(xml_file, xsd_file=None):
    """
    XML dosyasını XSD şemasına göre doğrular
    XSD yoksa basit yapısal doğrulama yapar
    """
    try:
        # XML dosyasını parse et
        tree = ET.parse(xml_file)
        root = tree.getroot()
        
        print(f"✅ XML parse edildi: {xml_file}")
        
        # Basit yapısal doğrulama
        validation_errors = []
        
        # Gerekli alanları kontrol et
        required_fields = {
            'Currency': 'Para birimi eksik',
            'Supplier': 'Tedarikçi bilgisi eksik',
            'Customer': 'Müşteri bilgisi eksik',
            'Items': 'Ürün listesi eksik',
            'Totals': 'Toplam bilgileri eksik'
        }
        
        for field, error_msg in required_fields.items():
            if root.find(field) is None:
                validation_errors.append(f"❌ {error_msg}: {field}")
            else:
                print(f"✅ {field} alanı mevcut")
        
        # Supplier alt alanları
        supplier = root.find('Supplier')
        if supplier is not None:
            if supplier.find('BIN') is None:
                validation_errors.append("❌ Supplier BIN eksik")
            else:
                bin_value = supplier.find('BIN').text
                if not bin_value:
                    validation_errors.append("❌ Supplier BIN boş")
                elif len(bin_value) != 12:
                    validation_errors.append(f"❌ Supplier BIN 12 hane olmalı: {bin_value} ({len(bin_value)} hane)")
                else:
                    print(f"✅ Supplier BIN: {bin_value}")
        
        # Customer alt alanları
        customer = root.find('Customer')
        if customer is not None:
            if customer.find('TaxNumber') is None:
                validation_errors.append("❌ Customer TaxNumber eksik")
            else:
                tax_value = customer.find('TaxNumber').text
                if not tax_value:
                    validation_errors.append("❌ Customer TaxNumber boş")
                elif len(tax_value) != 12:
                    validation_errors.append(f"❌ Customer TaxNumber 12 hane olmalı: {tax_value} ({len(tax_value)} hane)")
                else:
                    print(f"✅ Customer TaxNumber: {tax_value}")
        
        # Items kontrolü
        items = root.find('Items')
        if items is not None:
            item_count = len(items.findall('Item'))
            if item_count == 0:
                validation_errors.append("❌ Hiç ürün bulunamadı")
            else:
                print(f"✅ {item_count} adet ürün bulundu")
                
                # İlk ürünü detay kontrol et
                first_item = items.find('Item')
                if first_item is not None:
                    required_item_fields = ['Name', 'Quantity', 'UnitCode', 'UnitPrice']
                    for field in required_item_fields:
                        if first_item.find(field) is None:
                            validation_errors.append(f"❌ Ürün {field} alanı eksik")
                        else:
                            print(f"✅ Ürün {field}: {first_item.find(field).text}")
        
        # XSD şema doğrulaması (varsa)
        if xsd_file and os.path.exists(xsd_file):
            try:
                import lxml.etree as lxml_etree
                schema_doc = lxml_etree.parse(xsd_file)
                schema = lxml_etree.XMLSchema(schema_doc)
                
                xml_doc = lxml_etree.parse(xml_file)
                schema.assertValid(xml_doc)
                print(f"✅ XSD şema doğrulaması başarılı: {xsd_file}")
                
            except ImportError:
                print("⚠️ lxml kütüphanesi bulunamadı, XSD doğrulaması atlandı")
                print("   Kurulum: pip install lxml")
            except Exception as e:
                validation_errors.append(f"❌ XSD doğrulama hatası: {e}")
        else:
            print("⚠️ XSD şema dosyası bulunamadı, sadece yapısal doğrulama yapıldı")
        
        # Sonuçları raporla
        if validation_errors:
            print(f"\n❌ {len(validation_errors)} doğrulama hatası bulundu:")
            for error in validation_errors:
                print(f"  {error}")
            return False
        else:
            print(f"\n✅ Tüm doğrulamalar başarılı!")
            return True
            
    except ET.ParseError as e:
        print(f"❌ XML parse hatası: {e}")
        return False
    except Exception as e:
        print(f"❌ Doğrulama hatası: {e}")
        return False

def main():
    if len(sys.argv) < 2:
        print("Kullanım: python3 kz_xsd_validator.py <xml_file> [xsd_file]")
        print("Örnek: python3 kz_xsd_validator.py invoice.xml schema.xsd")
        sys.exit(1)
    
    xml_file = sys.argv[1]
    xsd_file = sys.argv[2] if len(sys.argv) > 2 else None
    
    if not os.path.exists(xml_file):
        print(f"❌ XML dosyası bulunamadı: {xml_file}")
        sys.exit(1)
    
    print(f"🔍 KZ XML Doğrulama: {xml_file}")
    if xsd_file:
        print(f"📋 XSD Şema: {xsd_file}")
    print("-" * 50)
    
    success = validate_xml_against_xsd(xml_file, xsd_file)
    
    if success:
        print("\n🎯 XML dosyası geçerli!")
        sys.exit(0)
    else:
        print("\n💥 XML dosyasında hatalar var!")
        sys.exit(1)

if __name__ == "__main__":
    main()
