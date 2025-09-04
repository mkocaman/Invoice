#!/usr/bin/env python3
"""
Test Data Normalizer
Test senaryolarını KZ XML ve UZ JSON formatlarına çevirir
"""

import sys
import os
import json
from pathlib import Path
from datetime import datetime

def normalize_kz_xml(scenario_data):
    """
    KZ test senaryosunu XML formatına çevirir
    """
    xml_content = f'''<?xml version="1.0" encoding="utf-8"?>
<Invoice>
  <DocumentType>{scenario_data.get('documentType', 'KZ-LOCAL')}</DocumentType>
  <Number>{scenario_data.get('invoiceNumber', 'TEST-KZ-001')}</Number>
  <Date>{scenario_data.get('issueDate', '2025-09-03')}</Date>
  <Currency>{scenario_data.get('currency', 'KZT')}</Currency>
  <Supplier>
    <BIN>{scenario_data.get('supplier', {}).get('bin', '123456789012')}</BIN>
    <Name>{scenario_data.get('supplier', {}).get('name', 'Test Supplier')}</Name>
    <Address>{scenario_data.get('supplier', {}).get('address', 'Test Address')}</Address>
    <Phone>{scenario_data.get('supplier', {}).get('phone', '+7-701-123-4567')}</Phone>
  </Supplier>
  <Customer>
    <BIN>{scenario_data.get('customer', {}).get('bin', '210987654321')}</BIN>
    <Name>{scenario_data.get('customer', {}).get('name', 'Test Customer')}</Name>
    <Address>{scenario_data.get('customer', {}).get('address', 'Test Customer Address')}</Address>
    <Phone>{scenario_data.get('customer', {}).get('phone', '+7-717-987-6543')}</Phone>
  </Customer>
  <Items>'''
    
    for item in scenario_data.get('items', []):
        xml_content += f'''
    <Item>
      <ID>{item.get('id', 1)}</ID>
      <Name>{item.get('name', 'Test Item')}</Name>
      <Description>{item.get('description', 'Test Description')}</Description>
      <Quantity>{item.get('quantity', 1)}</Quantity>
      <UnitCode>{item.get('unitCode', 'C62')}</UnitCode>
      <UnitPrice>{item.get('unitPrice', '0.00')}</UnitPrice>
      <LineExtensionAmount>{item.get('lineExtensionAmount', '0.00')}</LineExtensionAmount>
      <VatPercent>{item.get('vatPercent', '12')}</VatPercent>
      <VatAmount>{item.get('vatAmount', '0.00')}</VatAmount>
      <TotalAmount>{item.get('totalAmount', '0.00')}</TotalAmount>
    </Item>'''
    
    xml_content += f'''
  </Items>
  <Totals>
    <LineExtensionAmount>{scenario_data.get('totals', {}).get('lineExtensionAmount', '0.00')}</LineExtensionAmount>
    <TaxExclusiveAmount>{scenario_data.get('totals', {}).get('taxExclusiveAmount', '0.00')}</TaxExclusiveAmount>
    <TaxAmount>{scenario_data.get('totals', {}).get('taxAmount', '0.00')}</TaxAmount>
    <TaxInclusiveAmount>{scenario_data.get('totals', {}).get('taxInclusiveAmount', '0.00')}</TaxInclusiveAmount>
    <PayableAmount>{scenario_data.get('totals', {}).get('payableAmount', '0.00')}</PayableAmount>
  </Totals>
  <PaymentTerms>{scenario_data.get('paymentTerms', 'Net 30')}</PaymentTerms>
  <Notes>{scenario_data.get('notes', 'Test invoice')}</Notes>
</Invoice>'''
    
    return xml_content

def normalize_uz_json(scenario_data):
    """
    UZ test senaryosunu JSON formatına çevirir
    """
    json_data = {
        "scenario": scenario_data.get('scenario', 'UZ-TEST'),
        "invoiceNumber": scenario_data.get('invoiceNumber', 'TEST-UZ-001'),
        "currency": scenario_data.get('currency', 'UZS'),
        "sellerInn": scenario_data.get('supplier', {}).get('inn', '123456789'),
        "buyerInn": scenario_data.get('customer', {}).get('inn', '987654321'),
        "issueDate": scenario_data.get('issueDate', '2025-09-03'),
        "dueDate": scenario_data.get('dueDate', '2025-10-03'),
        "sellerName": scenario_data.get('supplier', {}).get('name', 'Test Supplier'),
        "buyerName": scenario_data.get('customer', {}).get('name', 'Test Customer'),
        "sellerAddress": scenario_data.get('supplier', {}).get('address', 'Test Address'),
        "buyerAddress": scenario_data.get('customer', {}).get('address', 'Test Customer Address'),
        "sellerPhone": scenario_data.get('supplier', {}).get('phone', '+998-71-123-4567'),
        "buyerPhone": scenario_data.get('customer', {}).get('phone', '+998-66-987-6543'),
        "items": []
    }
    
    for item in scenario_data.get('items', []):
        json_data["items"].append({
            "id": item.get('id', 1),
            "name": item.get('name', 'Test Item'),
            "description": item.get('description', 'Test Description'),
            "quantity": str(item.get('quantity', 1)),
            "unit_code": item.get('unitCode', 'C62'),
            "unit_price": str(item.get('unitPrice', '0.00')),
            "line_total": str(item.get('lineExtensionAmount', '0.00')),
            "vat_percent": str(item.get('vatPercent', '15')),
            "vat_amount": str(item.get('vatAmount', '0.00'))
        })
    
    json_data.update({
        "net": scenario_data.get('totals', {}).get('lineExtensionAmount', '0.00'),
        "vat": scenario_data.get('totals', {}).get('taxAmount', '0.00'),
        "gross": scenario_data.get('totals', {}).get('taxInclusiveAmount', '0.00'),
        "paymentTerms": scenario_data.get('paymentTerms', 'Net 30'),
        "notes": scenario_data.get('notes', 'Test invoice')
    })
    
    return json.dumps(json_data, ensure_ascii=False, indent=2)

def main():
    """
    Ana fonksiyon
    """
    if len(sys.argv) < 2:
        print("Kullanım: python3 test-normalizer.py <input_dir>")
        print("Örnek: python3 test-normalizer.py input/test-scenarios/")
        sys.exit(1)
    
    input_dir = sys.argv[1]
    
    if not os.path.exists(input_dir):
        print(f"❌ Input dizini bulunamadı: {input_dir}")
        sys.exit(1)
    
    print(f"🔧 Test Data Normalizer: {input_dir}")
    print("-" * 50)
    
    # Output dizinlerini oluştur
    os.makedirs("output/TEST_INVOICES", exist_ok=True)
    
    # Test senaryolarını bul
    scenario_files = []
    for pattern in ["*.json"]:
        scenario_files.extend(Path(input_dir).glob(pattern))
    
    if not scenario_files:
        print("⚠️ Test senaryosu bulunamadı")
        sys.exit(0)
    
    print(f"📁 {len(scenario_files)} test senaryosu bulundu")
    
    # Her senaryoyu normalize et
    for scenario_file in scenario_files:
        print(f"\n🔍 Normalize ediliyor: {scenario_file.name}")
        
        try:
            with open(scenario_file, 'r', encoding='utf-8') as f:
                scenario_data = json.load(f)
            
            scenario_name = scenario_data.get('scenario', scenario_file.stem)
            
            # KZ XML oluştur
            if scenario_data.get('documentType', '').startswith('KZ'):
                xml_content = normalize_kz_xml(scenario_data)
                xml_file = f"output/TEST_INVOICES/{scenario_name}.xml"
                
                with open(xml_file, 'w', encoding='utf-8') as f:
                    f.write(xml_content)
                
                print(f"   ✅ KZ XML: {xml_file}")
            
            # UZ JSON oluştur
            if scenario_data.get('documentType', '').startswith('UZ'):
                json_content = normalize_uz_json(scenario_data)
                json_file = f"output/TEST_INVOICES/{scenario_name}.json"
                
                with open(json_file, 'w', encoding='utf-8') as f:
                    f.write(json_content)
                
                print(f"   ✅ UZ JSON: {json_file}")
            
            # Hatalı senaryo için her iki format da oluştur
            if scenario_data.get('documentType') == 'INVALID':
                # KZ XML (hatalı)
                xml_content = normalize_kz_xml(scenario_data)
                xml_file = f"output/TEST_INVOICES/{scenario_name}_KZ.xml"
                
                with open(xml_file, 'w', encoding='utf-8') as f:
                    f.write(xml_content)
                
                print(f"   ✅ Hatalı KZ XML: {xml_file}")
                
                # UZ JSON (hatalı)
                json_content = normalize_uz_json(scenario_data)
                json_file = f"output/TEST_INVOICES/{scenario_name}_UZ.json"
                
                with open(json_file, 'w', encoding='utf-8') as f:
                    f.write(json_content)
                
                print(f"   ✅ Hatalı UZ JSON: {json_file}")
                
        except Exception as e:
            print(f"   ❌ Hata: {e}")
    
    print(f"\n🎯 Normalization tamamlandı!")
    print(f"📁 Çıktılar: output/TEST_INVOICES/")
    
    # Oluşturulan dosyaları listele
    print(f"\n📋 Oluşturulan dosyalar:")
    for file_path in Path("output/TEST_INVOICES").glob("*"):
        print(f"   📄 {file_path.name}")

if __name__ == "__main__":
    main()
