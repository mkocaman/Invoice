// Türkçe Açıklama:
// SandboxRunner bu endpoint'e POST ederek her olayı (SENT/ACK/NACK/DELIVERED...) bildirir.
// Controller, hem Audit hem StatusHistory kayıtlarını oluşturur.

using Infrastructure.Db.Entities;
using Infrastructure.Db.Services;
using Infrastructure.Providers.Core;
using Infrastructure.Providers.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceAuditService _audit;

    public InvoicesController(IInvoiceAuditService audit)
    {
        _audit = audit;
    }

    [HttpPost("audit")]
    public async Task<IActionResult> Audit([FromBody] InvoiceEventDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.InvoiceId) || string.IsNullOrWhiteSpace(dto.EventType))
            return BadRequest("InvoiceId ve EventType zorunludur.");

        // Korelasyon bilgisi
        var correlationId = HttpContext.Response.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? HttpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault();

        // Simulation flag: dto > env > false
        var simulation = dto.Simulation
            ?? (bool.TryParse(Environment.GetEnvironmentVariable("SANDBOX_SIMULATION"), out var sim) && sim);

        var audit = new InvoiceAudit
        {
            InvoiceId = dto.InvoiceId,
            ExternalInvoiceNumber = dto.ExternalInvoiceNumber,
            EventType = dto.EventType,
            StatusFrom = dto.StatusFrom,
            StatusTo = dto.StatusTo,
            SystemCode = dto.SystemCode,
            CorrelationId = correlationId,
            TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString(),
            XmlPayload = dto.XmlPayload,
            JsonPayload = dto.JsonPayload,
            RequestBody = dto.RequestBody,
            ResponseBody = dto.ResponseBody,
            Simulation = simulation,
            Actor = "api:invoices",
            Notes = dto.Notes
        };

        var id = await _audit.LogWithStatusAsync(
            entry: audit,
            invoiceId: dto.InvoiceId,
            eventType: dto.EventType,
            statusFrom: dto.StatusFrom,
            statusTo: dto.StatusTo,
            systemCode: dto.SystemCode,
            simulation: simulation,
            externalInvoiceNumber: dto.ExternalInvoiceNumber,
            eventKey: dto.EventKey,
            occurredAtUtc: dto.OccurredAtUtc ?? DateTime.UtcNow
        );

        return Ok(new { message = "Invoice event kaydedildi", id, dto.InvoiceId, dto.EventType });
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromQuery] string provider, [FromBody] dynamic body, [FromServices] IProviderFactory factory)
    {
        // Türkçe: Fatura içeriği (örnek) — gerçek projede DTO kullan
        string invoiceId = Guid.NewGuid().ToString("n");
        string? ublXml = (string?)body?.ublXml;
        string? jsonPayload = (string?)body?.jsonPayload;

        var p = factory.Get(provider);
        await p.AuthenticateAsync();

        var result = await p.SendAsync(invoiceId, ublXml, jsonPayload);
        // Türkçe: Audit + Status kayıtları (SENT/ACK/NACK mantığı)
        var simulation = true;

        var audit = new InvoiceAudit {
            InvoiceId = invoiceId,
            ExternalInvoiceNumber = result.ExternalInvoiceNumber,
            EventType = result.Success ? "SENT" : "ERROR",
            StatusFrom = result.Success ? "SIGNED" : "SIGNED",
            StatusTo = result.Success ? "SENT" : "ERROR",
            SystemCode = "TR",
            XmlPayload = ublXml,
            JsonPayload = jsonPayload,
            RequestBody = result.RawRequest,
            ResponseBody = result.RawResponse,
            Simulation = simulation,
            Actor = $"api:send:{provider}",
            Notes = result.ErrorMessage
        };
        var id = await _audit.LogWithStatusAsync(audit, invoiceId, audit.EventType, audit.StatusFrom, audit.StatusTo, audit.SystemCode, simulation, result.ExternalInvoiceNumber, eventKey: $"{provider}-{invoiceId}-sent");

        return Ok(new {
            message = "Gönderim tamamlandı",
            provider,
            invoiceId,
            external = result.ExternalInvoiceNumber,
            success = result.Success,
            error = result.ErrorMessage
        });
    }
}
