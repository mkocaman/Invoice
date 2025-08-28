namespace Invoice.Domain.Enums;

/// <summary>
/// İmzalama modu - ProviderSign veya SelfSign
/// Enum tekilleştirildi; tüm katmanlar SignMode kullanır.
/// </summary>
public enum SignMode
{
    /// <summary>
    /// Entegratör imzalama - JSON format ile entegratöre gönderilir
    /// </summary>
    ProviderSign = 1,
    
    /// <summary>
    /// Kendi imzalama - UBL XML oluşturulur ve XAdES-BES ile imzalanır
    /// </summary>
    SelfSign = 2
}
