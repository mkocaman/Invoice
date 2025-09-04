using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Invoice.SandboxRunner;

public interface IKzEsfClient
{
    Task<string> AuthenticateAsync(CancellationToken cancellationToken = default);
    Task<string> SendInvoiceAsync(string xmlContent, string token, CancellationToken cancellationToken = default);
}

public interface IUzDidoxClient
{
    Task<string> AuthenticateAsync(CancellationToken cancellationToken = default);
    Task<string> SendInvoiceAsync(string jsonContent, string token, CancellationToken cancellationToken = default);
}

public class KzEsfClient : IKzEsfClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KzEsfClient> _logger;
    private readonly KzOptions _options;

    public KzEsfClient(HttpClient httpClient, ILogger<KzEsfClient> logger, IOptions<KzOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<string> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("KZ authentication başlatılıyor...");
            
            var authRequest = new
            {
                username = _options.Username,
                password = _options.Password,
                grant_type = "password"
            };
            
            var json = JsonSerializer.Serialize(authRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(_options.AuthEndpoint, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation("KZ auth response: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("KZ authentication başarılı");
                return responseContent;
            }
            else
            {
                _logger.LogError("KZ authentication başarısız: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return responseContent;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "KZ authentication hatası");
            throw;
        }
    }

    public async Task<string> SendInvoiceAsync(string xmlContent, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("KZ invoice gönderimi başlatılıyor...");
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");
            
            var response = await _httpClient.PostAsync(_options.InvoiceEndpoint, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation("KZ invoice response: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("KZ invoice gönderimi başarılı");
                return responseContent;
            }
            else
            {
                // Türkçe: Structured logging parametreleri; placeholder kapatıldı
                _logger.LogError("KZ invoice gönderimi başarısız. StatusCode={StatusCode} Content={Content}", response.StatusCode, responseContent);
                return responseContent;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "KZ invoice gönderimi hatası");
            throw;
        }
    }
}

public class UzDidoxClient : IUzDidoxClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UzDidoxClient> _logger;
    private readonly UzOptions _options;

    public UzDidoxClient(HttpClient httpClient, ILogger<UzDidoxClient> logger, IOptions<UzOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<string> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("UZ authentication başlatılıyor...");
            
            var authRequest = new
            {
                pinfl = "123456789", // TODO: Gerçek PINFL
                eimzoSignature = $"base64_encoded_signature_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                clientId = _options.ClientId,
                clientSecret = _options.ClientSecret
            };
            
            var json = JsonSerializer.Serialize(authRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(_options.AuthEndpoint, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation("UZ auth response: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("UZ authentication başarılı");
                return responseContent;
            }
            else
            {
                _logger.LogError("UZ authentication başarısız: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return responseContent;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UZ authentication hatası");
            throw;
        }
    }

    public async Task<string> SendInvoiceAsync(string jsonContent, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("UZ invoice gönderimi başlatılıyor...");
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(_options.InvoiceEndpoint, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation("UZ invoice response: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("UZ invoice gönderimi başarılı");
                return responseContent;
            }
            else
            {
                // Türkçe: Structured logging parametreleri; placeholder kapatıldı
                _logger.LogError("UZ invoice gönderimi başarısız. StatusCode={StatusCode} Content={Content}", response.StatusCode, responseContent);
                return responseContent;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UZ invoice gönderimi hatası");
            throw;
        }
    }
}
