using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Mock UBL validation servisi - gerçek validasyon yapmaz
/// </summary>
public class MockUblValidationService : IUblValidationService
{
    private readonly ILogger<MockUblValidationService> _logger;

    public MockUblValidationService(ILogger<MockUblValidationService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// UBL XML validasyonu yapar (mock - her zaman geçerli döner)
    /// </summary>
    public async Task<ValidationResult> ValidateUblAsync(string ublXml, string schemaVersion = "2.1")
    {
        _logger.LogInformation("Mock UBL validasyonu yapılıyor. Schema: {SchemaVersion}", schemaVersion);
        
        // Mock validasyon - her zaman başarılı
        await Task.Delay(10); // Simüle edilmiş işlem süresi
        
        return new ValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            Warnings = new List<string>()
        };
    }
}
