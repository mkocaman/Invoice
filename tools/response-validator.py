#!/usr/bin/env python3
"""
Response Validation Script v2
Sandbox response'larını doğrular ve detaylı rapor üretir
"""

import sys
import os
import json
import glob
from pathlib import Path
from datetime import datetime

def validate_response_file(file_path):
    """
    Response dosyasını doğrular
    """
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read().strip()
        
        if not content:
            return {"valid": False, "error": "Empty file"}
        
        # CURL_ERROR kontrolü
        if "CURL_ERROR" in content:
            return {
                "valid": False,
                "file_path": file_path,
                "status": "error",
                "errors": ["CURL error occurred - network or endpoint issue"],
                "warnings": [],
                "invoice_id": None,
                "token": None,
                "response_type": "ERROR",
                "has_error_details": True
            }
        
        # JSON response kontrolü
        try:
            data = json.loads(content)
            return validate_json_response(data, file_path)
        except json.JSONDecodeError:
            # XML response kontrolü
            return validate_xml_response(content, file_path)
            
    except Exception as e:
        return {"valid": False, "error": f"File read error: {e}"}

def validate_json_response(data, file_path):
    """
    JSON response'ı doğrular
    """
    errors = []
    warnings = []
    
    # Status kontrolü
    if "status" not in data:
        errors.append("Missing 'status' field")
    else:
        status = data["status"]
        if status not in ["success", "fail", "error"]:
            warnings.append(f"Unexpected status value: {status}")
    
    # Error code kontrolü (hatalı senaryolarda zorunlu)
    if data.get("status") in ["fail", "error"]:
        if "error" not in data:
            errors.append("Error field required for failed responses")
        else:
            error_info = data["error"]
            if isinstance(error_info, dict):
                if "code" not in error_info:
                    errors.append("Error code required for failed responses")
                if "message" not in error_info:
                    warnings.append("Error message recommended for failed responses")
            else:
                warnings.append("Error should be an object with code and message")
    
    # Invoice ID kontrolü (başarılı senaryolarda zorunlu)
    invoice_id = None
    if data.get("status") == "success" and "data" in data and isinstance(data["data"], dict):
        if "invoiceId" in data["data"]:
            invoice_id = data["data"]["invoiceId"]
        elif "id" in data["data"]:
            invoice_id = data["data"]["id"]
        
        if not invoice_id:
            errors.append("Invoice ID required for successful responses")
    
    # Token kontrolü
    token = None
    if "data" in data and isinstance(data["data"], dict):
        if "token" in data["data"]:
            token = data["data"]["token"]
    
    return {
        "valid": len(errors) == 0,
        "file_path": file_path,
        "status": data.get("status", "unknown"),
        "errors": errors,
        "warnings": warnings,
        "invoice_id": invoice_id,
        "token": token,
        "response_type": "JSON",
        "has_error_details": "error" in data and isinstance(data["error"], dict)
    }

def validate_xml_response(content, file_path):
    """
    XML response'ı doğrular (basit kontrol)
    """
    errors = []
    warnings = []
    
    # Basit XML kontrolü
    if not content.startswith("<?xml") and not content.startswith("<"):
        errors.append("Not a valid XML format")
    
    # Status kontrolü (basit string search)
    status = None
    if "success" in content.lower():
        status = "success"
    elif "fail" in content.lower() or "error" in content.lower():
        status = "fail"
    
    # Invoice ID kontrolü (basit string search)
    invoice_id = None
    if "invoiceId" in content or "invoice_id" in content:
        # Basit regex benzeri arama
        import re
        match = re.search(r'<[^>]*>([^<]*invoice[^<]*)</[^>]*>', content, re.IGNORECASE)
        if match:
            invoice_id = match.group(1).strip()
    
    # Error details kontrolü
    has_error_details = "error" in content.lower() or "code" in content.lower()
    
    return {
        "valid": len(errors) == 0,
        "file_path": file_path,
        "status": status or "unknown",
        "errors": errors,
        "warnings": warnings,
        "invoice_id": invoice_id,
        "token": None,
        "response_type": "XML",
        "has_error_details": has_error_details
    }

def generate_error_report(validation_results):
    """
    Error raporu oluşturur
    """
    error_report = []
    
    for result in validation_results:
        if not result["valid"] or result["errors"]:
            error_report.append({
                "file": os.path.basename(result["file_path"]),
                "status": result["status"],
                "errors": result["errors"],
                "warnings": result["warnings"],
                "response_type": result["response_type"],
                "has_error_details": result.get("has_error_details", False)
            })
    
    return error_report

def generate_success_report(validation_results):
    """
    Başarı raporu oluşturur
    """
    success_report = []
    
    for result in validation_results:
        if result["valid"] and result["status"] == "success":
            success_report.append({
                "file": os.path.basename(result["file_path"]),
                "invoice_id": result["invoice_id"],
                "token": result["token"],
                "response_type": result["response_type"]
            })
    
    return success_report

def main():
    """
    Ana fonksiyon
    """
    if len(sys.argv) < 2:
        print("Kullanım: python3 response-validator.py <output_dir>")
        print("Örnek: python3 response-validator.py output/")
        sys.exit(1)
    
    output_dir = sys.argv[1]
    
    if not os.path.exists(output_dir):
        print(f"❌ Output dizini bulunamadı: {output_dir}")
        sys.exit(1)
    
    print(f"🔍 Response Validation v2: {output_dir}")
    print("-" * 50)
    
    # Response dosyalarını bul
    response_files = []
    for pattern in ["*_RESPONSE_*.json", "*_ERROR_*.json", "*_AUTH_*.json"]:
        response_files.extend(glob.glob(os.path.join(output_dir, pattern)))
    
    if not response_files:
        print("⚠️ Response dosyası bulunamadı")
        sys.exit(0)
    
    print(f"📁 {len(response_files)} response dosyası bulundu")
    
    # Her dosyayı doğrula
    validation_results = []
    for file_path in response_files:
        print(f"\n🔍 Doğrulanıyor: {os.path.basename(file_path)}")
        result = validate_response_file(file_path)
        validation_results.append(result)
        
        if result["valid"] and not result["errors"]:
            print(f"✅ Geçerli response")
            if result["invoice_id"]:
                print(f"   📄 Invoice ID: {result['invoice_id']}")
            if result["token"]:
                print(f"   🔐 Token: {result['token'][:20]}...")
        else:
            print(f"❌ Validation hatası")
            for error in result["errors"]:
                print(f"   ❌ {error}")
            for warning in result["warnings"]:
                print(f"   ⚠️ {warning}")
    
    # Error raporu oluştur
    error_report = generate_error_report(validation_results)
    success_report = generate_success_report(validation_results)
    
    if error_report:
        print(f"\n📋 Error Raporu ({len(error_report)} dosya):")
        for error in error_report:
            print(f"   📄 {error['file']} ({error['response_type']}) - {error['status']}")
            for err in error["errors"]:
                print(f"      ❌ {err}")
        
        # Error log dosyasına yaz
        error_log_path = "docs/reports/SANDBOX_ERRORS.md"
        os.makedirs(os.path.dirname(error_log_path), exist_ok=True)
        
        with open(error_log_path, "w", encoding="utf-8") as f:
            f.write("# Sandbox Error Log v2\n\n")
            f.write(f"Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
            
            for error in error_report:
                f.write(f"## {error['file']}\n")
                f.write(f"- **Type**: {error['response_type']}\n")
                f.write(f"- **Status**: {error['status']}\n")
                f.write(f"- **Errors**:\n")
                for err in error["errors"]:
                    f.write(f"  - {err}\n")
                if error["warnings"]:
                    f.write(f"- **Warnings**:\n")
                    for warning in error["warnings"]:
                        f.write(f"  - {warning}\n")
                f.write("\n")
        
        print(f"\n📝 Error log kaydedildi: {error_log_path}")
    else:
        print(f"\n✅ Tüm response'lar geçerli!")
    
    # Başarı raporu
    if success_report:
        print(f"\n🎯 Başarı Raporu ({len(success_report)} dosya):")
        for success in success_report:
            print(f"   📄 {success['file']} - Invoice ID: {success['invoice_id'] or 'N/A'}")
    
    # Final status raporu oluştur
    status_report_path = "docs/STATUS_SANDBOX_RESULTS.md"
    os.makedirs(os.path.dirname(status_report_path), exist_ok=True)
    
    with open(status_report_path, "w", encoding="utf-8") as f:
        f.write("# STATUS — Sandbox Test Results v2\n\n")
        f.write(f"Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
        
        # Genel istatistikler
        total_count = len(validation_results)
        valid_count = sum(1 for r in validation_results if r["valid"])
        success_count = sum(1 for r in validation_results if r["status"] == "success")
        error_count = total_count - valid_count
        
        f.write("## 📊 Genel İstatistikler\n\n")
        f.write(f"- **Toplam Response**: {total_count}\n")
        f.write(f"- **Geçerli Response**: {valid_count}\n")
        f.write(f"- **Başarılı Response**: {success_count}\n")
        f.write(f"- **Hatalı Response**: {error_count}\n")
        f.write(f"- **Başarı Oranı**: {(valid_count/total_count*100):.1f}%\n\n")
        
        # Başarılı senaryolar
        if success_report:
            f.write("## ✅ Başarılı Senaryolar\n\n")
            for success in success_report:
                f.write(f"### {success['file']}\n")
                f.write(f"- **Invoice ID**: {success['invoice_id'] or 'N/A'}\n")
                f.write(f"- **Token**: {'Alındı' if success['token'] else 'N/A'}\n")
                f.write(f"- **Type**: {success['response_type']}\n\n")
        
        # Hatalı senaryolar
        if error_report:
            f.write("## ❌ Hatalı Senaryolar\n\n")
            for error in error_report:
                f.write(f"### {error['file']}\n")
                f.write(f"- **Status**: {error['status']}\n")
                f.write(f"- **Type**: {error['response_type']}\n")
                f.write(f"- **Errors**:\n")
                for err in error["errors"]:
                    f.write(f"  - {err}\n")
                if error["warnings"]:
                    f.write(f"- **Warnings**:\n")
                    for warning in error["warnings"]:
                        f.write(f"  - {warning}\n")
                f.write("\n")
        
        # Sonraki adımlar
        f.write("## 🚀 Sonraki Adımlar\n\n")
        if error_count > 0:
            f.write("1. **Error Analysis**: Hatalı response'ları analiz et\n")
            f.write("2. **Validation Rules**: Validation kurallarını gözden geçir\n")
            f.write("3. **API Testing**: Gerçek sandbox endpoint'lerini test et\n")
        else:
            f.write("1. **Production Ready**: Tüm testler başarılı\n")
            f.write("2. **Real Credentials**: Gerçek sandbox bilgilerini ekle\n")
            f.write("3. **Live Testing**: Canlı ortamda test et\n")
    
    print(f"\n📝 Status report kaydedildi: {status_report_path}")
    
    # Özet
    print(f"\n📊 Özet:")
    print(f"   ✅ Geçerli: {valid_count}/{total_count}")
    print(f"   ❌ Hatalı: {error_count}/{total_count}")
    print(f"   🎯 Başarı Oranı: {(valid_count/total_count*100):.1f}%")
    
    if valid_count == total_count:
        print("\n🎯 Tüm response'lar başarıyla doğrulandı!")
        sys.exit(0)
    else:
        print(f"\n💥 {error_count} response'da hata bulundu!")
        sys.exit(1)

if __name__ == "__main__":
    main()
