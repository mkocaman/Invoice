using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// UBL XML şemalarını çözen servis
/// </summary>
public class UblSchemaResolver : IUblSchemaResolver
{
    private readonly ILogger<UblSchemaResolver> _logger;
    private readonly Dictionary<string, string> _schemaCache;

    /// <summary>
    /// Constructor
    /// </summary>
    public UblSchemaResolver(ILogger<UblSchemaResolver> logger)
    {
        _logger = logger;
        _schemaCache = new Dictionary<string, string>();
    }

    /// <summary>
    /// UBL şema dosyasının yolunu alır
    /// </summary>
    public string GetSchemaPath(string schemaName)
    {
        _logger.LogDebug("UBL şema yolu alınıyor: {SchemaName}", schemaName);

        // Şema cache'den kontrol et
        if (_schemaCache.ContainsKey(schemaName))
        {
            return _schemaCache[schemaName];
        }

        // UBL şema dosyalarının yolu
        var schemaPath = schemaName.ToLower() switch
        {
            "invoice-2.1" => "Schemas/UBL-Invoice-2.1.xsd",
            "invoice-2.2" => "Schemas/UBL-Invoice-2.2.xsd",
            "invoice-2.3" => "Schemas/UBL-Invoice-2.3.xsd",
            "commonbasiccomponents-2" => "Schemas/UBL-CommonBasicComponents-2.xsd",
            "commonaggregatecomponents-2" => "Schemas/UBL-CommonAggregateComponents-2.xsd",
            "qualifieddatatypes-2" => "Schemas/UBL-QualifiedDataTypes-2.xsd",
            "unqualifieddatatypes-2" => "Schemas/UBL-UnqualifiedDataTypes-2.xsd",
            _ => throw new ArgumentException($"Desteklenmeyen UBL şema: {schemaName}")
        };

        // Cache'e ekle
        _schemaCache[schemaName] = schemaPath;

        _logger.LogDebug("UBL şema yolu alındı: {SchemaPath}", schemaPath);

        return schemaPath;
    }

    /// <summary>
    /// Şema dosyasının tam yolunu alır
    /// </summary>
    public string GetFullSchemaPath(string schemaName)
    {
        var relativePath = GetSchemaPath(schemaName);
        
        // Uygulama dizininden şema dosyasının tam yolunu oluştur
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var fullPath = Path.Combine(baseDirectory, relativePath);

        _logger.LogDebug("UBL şema tam yolu: {FullPath}", fullPath);

        return fullPath;
    }

    /// <summary>
    /// Şema dosyasının var olup olmadığını kontrol eder
    /// </summary>
    public bool SchemaExists(string schemaName)
    {
        var fullPath = GetFullSchemaPath(schemaName);
        var exists = File.Exists(fullPath);

        _logger.LogDebug("UBL şema dosyası kontrol edildi: {SchemaName} - Exists: {Exists}", schemaName, exists);

        return exists;
    }

    /// <summary>
    /// Tüm desteklenen şemaları listeler
    /// </summary>
    public IEnumerable<string> GetSupportedSchemas()
    {
        return new[]
        {
            "invoice-2.1",
            "invoice-2.2", 
            "invoice-2.3",
            "commonbasiccomponents-2",
            "commonaggregatecomponents-2",
            "qualifieddatatypes-2",
            "unqualifieddatatypes-2"
        };
    }
}
