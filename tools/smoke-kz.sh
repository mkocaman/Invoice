#!/usr/bin/env bash
set -Eeuo pipefail
export RPROMPT="" || true

echo "== KZ Sandbox Test v2 başlıyor =="
ROOT="$(pwd)"; echo "ROOT: $ROOT"
mkdir -p output docs logs output/responses

# Konfigürasyon yükle
source ./tools/sandbox-utils.sh
init_sandbox

# Parametre kontrolü
INVOICE_FILE="${1:-}"
if [[ -z "$INVOICE_FILE" ]]; then
    echo "❌ Kullanım: $0 <invoice_file>"
    echo "Örnek: $0 output/TEST_INVOICES/KZ_SIMPLE.xml"
    echo ""
    echo "Mevcut test dosyaları:"
    ls -1 output/TEST_INVOICES/KZ_*.xml 2>/dev/null || echo "Henüz test dosyası yok"
    exit 1
fi

if [[ ! -f "$INVOICE_FILE" ]]; then
    echo "❌ Dosya bulunamadı: $INVOICE_FILE"
    exit 1
fi

# Konfigürasyon
KZ_BASE_URL="https://esf-test.kgd.gov.kz"
AUTH_ENDPOINT="/api/auth/login"
INVOICE_ENDPOINT="/api/documents/invoice/send"

# Test credentials (appsettings'den okunmalı)
USERNAME="SANDBOX_KZ_USERNAME"
PASSWORD="SANDBOX_KZ_PASSWORD"

log_info "KZ Sandbox test başlatılıyor..."
log_info "Base URL: $KZ_BASE_URL"
log_info "Invoice File: $INVOICE_FILE"

echo "== Step 1: KZ Sandbox Auth Test =="
echo "Endpoint: POST $KZ_BASE_URL$AUTH_ENDPOINT"
echo "Status: Sandbox endpoint'e bağlanıyor..."

# Auth request with retry
log_info "Auth endpoint'e bağlanılıyor..."

    AUTH_RESPONSE=$(curl -s -w "\n%{http_code}" \
      -X POST "$KZ_BASE_URL$AUTH_ENDPOINT" \
      -H "Content-Type: application/json" \
      -d "{
        \"username\": \"$USERNAME\",
        \"password\": \"$PASSWORD\",
        \"grant_type\": \"password\"
      }" 2>/dev/null || echo "CURL_ERROR")

# Response parsing
HTTP_CODE=$(echo "$AUTH_RESPONSE" | tail -n1)
RESPONSE_BODY=$(echo "$AUTH_RESPONSE" | head -n -1 2>/dev/null || echo "$AUTH_RESPONSE")

if [[ "$HTTP_CODE" == "200" ]]; then
    log_success "Auth başarılı (HTTP 200)"
    TOKEN=$(echo "$RESPONSE_BODY" | jq -r '.data.token // empty' 2>/dev/null || echo "")
    
    if [[ -n "$TOKEN" && "$TOKEN" != "null" ]]; then
        log_success "Bearer token alındı"
        echo "$RESPONSE_BODY" > output/responses/KZ_AUTH_RESPONSE.json
        log_info "Token kaydedildi: output/responses/KZ_AUTH_RESPONSE.json"
        
        # Response validation
        if validate_json_response "$RESPONSE_BODY" "status" "data"; then
            log_success "Auth response validation passed"
        else
            log_warning "Auth response validation failed"
        fi
    else
        log_error "Token response'da bulunamadı"
        echo "$RESPONSE_BODY" > output/responses/KZ_AUTH_RESPONSE.json
    fi
else
    log_error "Auth başarısız (HTTP $HTTP_CODE)"
    echo "$RESPONSE_BODY" > output/responses/KZ_AUTH_RESPONSE.json
    log_info "Response kaydedildi: output/responses/KZ_AUTH_RESPONSE.json"
    
    # Error logging
    if [[ "$HTTP_CODE" != "200" ]]; then
        log_error "KZ Auth Error - HTTP $HTTP_CODE" >> logs/sandbox-errors.log
        echo "Response: $RESPONSE_BODY" >> logs/sandbox-errors.log
    fi
fi

echo "== Step 2: Invoice Send Endpoint Test =="
echo "Endpoint: POST $KZ_BASE_URL$INVOICE_ENDPOINT"
echo "Invoice File: $INVOICE_FILE"
echo "Status: Sandbox endpoint'e bağlanıyor..."

# Invoice dosyasını kontrol et
if [[ ! -f "$INVOICE_FILE" ]]; then
    log_error "Invoice dosyası bulunamadı: $INVOICE_FILE"
    exit 1
fi

log_info "Invoice dosyası yükleniyor: $INVOICE_FILE"
log_info "Dosya boyutu: $(wc -c < "$INVOICE_FILE") bytes"

# Invoice gönder (eğer token varsa)
TOKEN="${TOKEN:-}"
if [[ -n "$TOKEN" && "$TOKEN" != "null" ]]; then
    log_info "Bearer token ile invoice gönderiliyor..."
    
    INVOICE_RESPONSE=$(retry_with_backoff "$SANDBOX_MAX_RETRIES" "$SANDBOX_DELAY" "$SANDBOX_MULTIPLIER" \
      'curl -s -w "\n%{http_code}" \
        -X POST "$KZ_BASE_URL$INVOICE_ENDPOINT" \
        -H "Authorization: Bearer $TOKEN" \
        -H "Content-Type: application/xml" \
        --data-binary "@$INVOICE_FILE" 2>/dev/null || echo "CURL_ERROR"')
    
    INVOICE_HTTP_CODE=$(echo "$INVOICE_RESPONSE" | tail -n1)
    INVOICE_RESPONSE_BODY=$(echo "$INVOICE_RESPONSE" | head -n -1 2>/dev/null || echo "$INVOICE_RESPONSE")
    
    # HTTP status kontrolü
    if http_check "$INVOICE_HTTP_CODE"; then
        log_success "Invoice gönderimi başarılı (HTTP $INVOICE_HTTP_CODE)"
        
        # Response dosya adını oluştur
        BASE_NAME=$(basename "$INVOICE_FILE" .xml)
        RESPONSE_FILE="output/responses/SANDBOX_KZ_INVOICE_RESPONSE_${BASE_NAME}_$(date +%Y%m%d_%H%M%S).json"
        echo "$INVOICE_RESPONSE_BODY" > "$RESPONSE_FILE"
        log_info "Response kaydedildi: $RESPONSE_FILE"
        
        # Response validation
        if validate_json_response "$INVOICE_RESPONSE_BODY" "status" "data"; then
            log_success "Invoice response validation passed"
        else
            log_warning "Invoice response validation failed"
        fi
    else
        log_error "Invoice gönderimi başarısız (HTTP $INVOICE_HTTP_CODE)"
        
        # Error response dosya adını oluştur
        BASE_NAME=$(basename "$INVOICE_FILE" .xml)
        ERROR_FILE="output/responses/SANDBOX_KZ_INVOICE_ERROR_${BASE_NAME}_$(date +%Y%m%d_%H%M%S).json"
        echo "$INVOICE_RESPONSE_BODY" > "$ERROR_FILE"
        log_info "Error response kaydedildi: $ERROR_FILE"
        
        # Error logging
        log_error "KZ Invoice Error - HTTP $INVOICE_HTTP_CODE" >> logs/sandbox-errors.log
        echo "File: $INVOICE_FILE" >> logs/sandbox-errors.log
        echo "Response: $INVOICE_RESPONSE_BODY" >> logs/sandbox-errors.log
        echo "---" >> logs/sandbox-errors.log
    fi
else
    log_warning "Token olmadığı için invoice gönderimi atlandı"
fi

echo "== Step 3: Test Sonucu =="
echo "✅ KZ Sandbox test tamamlandı"
echo "📁 Invoice File: $INVOICE_FILE"
echo "📊 Response: output/responses/"

if [[ -n "$TOKEN" && "$TOKEN" != "null" ]]; then
    echo "🔐 Token: Alındı"
else
    echo "🔐 Token: Alınamadı"
fi

log_success "KZ Sandbox test tamamlandı - $INVOICE_FILE"
