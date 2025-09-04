namespace Invoice.Infrastructure.Providers.UZ;

public sealed record UzAuthRequest(string SignedData);
public sealed record UzAuthResponse(string AccessToken, string TokenType, int ExpiresIn);

public sealed record UzInvoiceLine(
    string name, string quantity, string unit_code, string unit_price, string line_total, string vat_percent, string vat_amount
);

public sealed record UzInvoicePayload(
    string invoiceNumber, string currency, string sellerInn, string buyerInn, string issueDate, List<UzInvoiceLine> items, string net, string vat, string gross
);

public sealed record UzInvoiceResponse(string id, string number, string status);
