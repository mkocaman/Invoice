namespace Invoice.Application.Interfaces;

/// <summary>
/// UBL şema çözümleme servisi sözleşmesi
/// </summary>
public interface IUblSchemaResolver
{
    /// <summary>
    /// Şema adına göre dosya yolunu döndürür
    /// </summary>
    /// <param name="schemaName">Şema adı</param>
    /// <returns>Göreceli dosya yolu</returns>
    string GetSchemaPath(string schemaName);
    
    /// <summary>
    /// Şema adına göre tam dosya yolunu döndürür
    /// </summary>
    /// <param name="schemaName">Şema adı</param>
    /// <returns>Tam dosya yolu</returns>
    string GetFullSchemaPath(string schemaName);
    
    /// <summary>
    /// Şema dosyasının var olup olmadığını kontrol eder
    /// </summary>
    /// <param name="schemaName">Şema adı</param>
    /// <returns>Dosya var mı?</returns>
    bool SchemaExists(string schemaName);
    
    /// <summary>
    /// Desteklenen şemaları listeler
    /// </summary>
    /// <returns>Şema adları listesi</returns>
    IEnumerable<string> GetSupportedSchemas();
}
