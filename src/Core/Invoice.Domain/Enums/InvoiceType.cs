namespace Invoice.Domain.Enums;

/// <summary>
/// Fatura tipleri
/// </summary>
public enum InvoiceType
{
    /// <summary>
    /// Satış faturası
    /// </summary>
    SATIS = 1,
    
    /// <summary>
    /// İade faturası
    /// </summary>
    IADE = 2,
    
    /// <summary>
    /// Düzeltme faturası
    /// </summary>
    DUZELTME = 3
}
