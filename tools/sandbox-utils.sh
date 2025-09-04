#!/usr/bin/env bash
# Sandbox Utility Functions
# Common functions for sandbox integration

set -Eeuo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
SANDBOX_MAX_RETRIES=${SANDBOX_MAX_RETRIES:-3}
SANDBOX_DELAY=${SANDBOX_DELAY:-2}
SANDBOX_MULTIPLIER=${SANDBOX_MULTIPLIER:-2.0}

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $(date '+%Y-%m-%d %H:%M:%S') - $1"
    # Error log dosyasına da yaz
    echo "$(date '+%Y-%m-%d %H:%M:%S') - ERROR: $1" >> logs/sandbox-errors.log
}

# HTTP response check function
http_check() {
    local http_code="$1"
    
    if [[ "$http_code" == "200" ]]; then
        return 0  # Success
    elif [[ "$http_code" == "201" ]]; then
        return 0  # Created
    elif [[ "$http_code" == "202" ]]; then
        return 0  # Accepted
    elif [[ "$http_code" == "400" ]]; then
        log_warning "Bad Request (HTTP 400)"
        return 1
    elif [[ "$http_code" == "401" ]]; then
        log_error "Unauthorized (HTTP 401)"
        return 1
    elif [[ "$http_code" == "403" ]]; then
        log_error "Forbidden (HTTP 403)"
        return 1
    elif [[ "$http_code" == "404" ]]; then
        log_error "Not Found (HTTP 404)"
        return 1
    elif [[ "$http_code" == "500" ]]; then
        log_error "Internal Server Error (HTTP 500)"
        return 1
    elif [[ "$http_code" == "502" ]]; then
        log_error "Bad Gateway (HTTP 502)"
        return 1
    elif [[ "$http_code" == "503" ]]; then
        log_error "Service Unavailable (HTTP 503)"
        return 1
    else
        log_warning "Unknown HTTP status: $http_code"
        return 1
    fi
}

# Retry with exponential backoff
retry_with_backoff() {
    local max_attempts=${1:-3}
    local delay=${2:-2}
    local backoff_multiplier=${3:-2.0}
    local attempt=1
    local current_delay=$delay

    shift 3
    local command="$@"

    log_info "Executing command with retry (max: $max_attempts, delay: ${delay}s, multiplier: $backoff_multiplier)"

    while [ $attempt -le $max_attempts ]; do
        log_info "Attempt $attempt/$max_attempts"

        if eval "$command"; then
            log_success "Command succeeded on attempt $attempt"
            return 0
        else
            local exit_code=$?
            log_warning "Command failed on attempt $attempt (exit code: $exit_code)"

            if [ $attempt -eq $max_attempts ]; then
                log_error "All $max_attempts attempts failed. Giving up."
                return $exit_code
            fi

            log_info "Waiting ${current_delay}s before retry..."
            sleep $current_delay

            # Exponential backoff calculation
            if command -v bc >/dev/null 2>&1; then
                current_delay=$(echo "$current_delay * $backoff_multiplier" | bc -l 2>/dev/null || echo "$((current_delay * 2))")
            else
                current_delay=$((current_delay * 2))
            fi
            
            attempt=$((attempt + 1))
        fi
    done

    return 1
}

# JSON response validation
validate_json_response() {
    local response="$1"
    local required_fields=("${@:2}")
    
    if [[ -z "$response" ]]; then
        log_error "Empty response"
        return 1
    fi
    
    # Check if response is valid JSON
    if ! echo "$response" | jq . >/dev/null 2>&1; then
        log_error "Invalid JSON response"
        return 1
    fi
    
    # Check required fields
    for field in "${required_fields[@]}"; do
        if ! echo "$response" | jq -e ".$field" >/dev/null 2>&1; then
            log_error "Missing required field: $field"
            return 1
        fi
    done
    
    log_success "JSON response validation passed"
    return 0
}

# Configuration loading
load_config() {
    if [[ -f "appsettings.Development.json" ]]; then
        log_info "Loading configuration from appsettings.Development.json"
        
        # Extract sandbox configuration using jq
        if command -v jq >/dev/null 2>&1; then
            SANDBOX_MAX_RETRIES=$(jq -r '.Sandbox.RetryPolicy.MaxRetries // 3' appsettings.Development.json 2>/dev/null || echo "3")
            SANDBOX_DELAY=$(jq -r '.Sandbox.RetryPolicy.DelaySeconds // 2' appsettings.Development.json 2>/dev/null || echo "2")
            SANDBOX_MULTIPLIER=$(jq -r '.Sandbox.RetryPolicy.BackoffMultiplier // 2.0' appsettings.Development.json 2>/dev/null || echo "2.0")
            
            log_info "Configuration loaded: MaxRetries=$SANDBOX_MAX_RETRIES, Delay=${SANDBOX_DELAY}s, Multiplier=$SANDBOX_MULTIPLIER"
        else
            log_warning "jq not available, using default configuration"
        fi
    else
        log_warning "appsettings.Development.json not found, using default configuration"
    fi
}

# Dependency checking
check_dependencies() {
    local missing_deps=()
    
    # Check required commands
    for cmd in curl jq; do
        if ! command -v "$cmd" >/dev/null 2>&1; then
            missing_deps+=("$cmd")
        fi
    done
    
    if [[ ${#missing_deps[@]} -gt 0 ]]; then
        log_error "Missing required dependencies: ${missing_deps[*]}"
        return 1
    fi
    
    log_success "All required dependencies available"
    return 0
}

# Sandbox environment initialization
init_sandbox() {
    log_info "Initializing sandbox environment..."
    
    # Create necessary directories
    mkdir -p logs output/responses
    
    # Check dependencies
    if ! check_dependencies; then
        log_error "Dependency check failed"
        exit 1
    fi
    
    # Load configuration
    load_config
    
    # Initialize error log
    touch logs/sandbox-errors.log
    
    log_success "Sandbox environment initialized"
}

# Test function
test_utils() {
    echo "== Testing Sandbox Utils =="
    
    # Test logging
    log_info "Testing info logging"
    log_success "Testing success logging"
    log_warning "Testing warning logging"
    log_error "Testing error logging"
    
    # Test HTTP check
    echo "Testing HTTP check function..."
    if http_check "200"; then
        echo "✅ HTTP 200 check passed"
    else
        echo "❌ HTTP 200 check failed"
    fi
    
    if http_check "500"; then
        echo "❌ HTTP 500 check should have failed"
    else
        echo "✅ HTTP 500 check correctly failed"
    fi
    
    # Test retry function (simulate failure)
    echo "Testing retry function (will fail 2 times then succeed)..."
    local attempt=0
    retry_with_backoff 3 1 2.0 '
        attempt=$((attempt + 1))
        if [ $attempt -lt 3 ]; then
            echo "Simulated failure attempt $attempt"
            return 1
        else
            echo "Simulated success on attempt $attempt"
            return 0
        fi
    '
    
    echo "== Utils test completed =="
}

# Main function
main() {
    case "${1:-}" in
        "test")
            test_utils
            ;;
        "init")
            init_sandbox
            ;;
        *)
            echo "Usage: $0 {test|init}"
            echo "  test - Test utility functions"
            echo "  init - Initialize sandbox environment"
            exit 1
            ;;
    esac
}

# Run main if script is executed directly
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
