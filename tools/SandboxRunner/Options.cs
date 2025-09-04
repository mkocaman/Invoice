namespace Invoice.SandboxRunner;

public class KzOptions
{
    public const string SectionName = "Sandbox:KZ";
    
    public string BaseUrl { get; set; } = string.Empty;
    public string AuthEndpoint { get; set; } = string.Empty;
    public string InvoiceEndpoint { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 60;
    
    // Environment variables'den alınacak
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? CertPath { get; set; }
    public string? CertPassword { get; set; }
}

public class UzOptions
{
    public const string SectionName = "Sandbox:UZ";
    
    public string BaseUrl { get; set; } = string.Empty;
    public string AuthEndpoint { get; set; } = string.Empty;
    public string InvoiceEndpoint { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    
    // Environment variables'den alınacak
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string? CertPath { get; set; }
    public string? CertPassword { get; set; }
}

public class RetryPolicyOptions
{
    public const string SectionName = "Sandbox:RetryPolicy";
    
    public int MaxRetries { get; set; } = 3;
    public int DelaySeconds { get; set; } = 2;
    public double BackoffMultiplier { get; set; } = 2.0;
}
