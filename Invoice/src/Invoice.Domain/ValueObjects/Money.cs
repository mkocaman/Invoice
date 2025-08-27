namespace Invoice.Domain.ValueObjects;

/// <summary>
/// Para value object'i
/// </summary>
public class Money
{
    /// <summary>
    /// Tutar
    /// </summary>
    public decimal Amount { get; private set; }
    
    /// <summary>
    /// Para birimi
    /// </summary>
    public string Currency { get; private set; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="amount">Tutar</param>
    /// <param name="currency">Para birimi</param>
    public Money(decimal amount, string currency = "TRY")
    {
        Amount = amount;
        Currency = currency;
    }
    
    /// <summary>
    /// Para ekleme
    /// </summary>
    /// <param name="other">Diğer para</param>
    /// <returns>Toplam para</returns>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Farklı para birimleri toplanamaz");
        
        return new Money(Amount + other.Amount, Currency);
    }
    
    /// <summary>
    /// Para çıkarma
    /// </summary>
    /// <param name="other">Diğer para</param>
    /// <returns>Fark para</returns>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Farklı para birimleri çıkarılamaz");
        
        return new Money(Amount - other.Amount, Currency);
    }
    
    /// <summary>
    /// Para çarpma
    /// </summary>
    /// <param name="multiplier">Çarpan</param>
    /// <returns>Çarpım para</returns>
    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }
    
    /// <summary>
    /// Para bölme
    /// </summary>
    /// <param name="divisor">Bölen</param>
    /// <returns>Bölüm para</returns>
    public Money Divide(decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException("Sıfıra bölme hatası");
        
        return new Money(Amount / divisor, Currency);
    }
    
    /// <summary>
    /// String'e çevirme
    /// </summary>
    /// <returns>Para string'i</returns>
    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
    
    /// <summary>
    /// Eşitlik kontrolü
    /// </summary>
    /// <param name="obj">Diğer obje</param>
    /// <returns>Eşit mi?</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Money other)
            return false;
        
        return Amount == other.Amount && Currency == other.Currency;
    }
    
    /// <summary>
    /// Hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }
}
