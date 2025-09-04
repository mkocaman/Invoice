using System.Text.Json;
using Infrastructure.Workflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Workflow;

namespace WebApi.Controllers.Workflow;

// Türkçe: İş akışı toparlayıcı uçlar (CSMS korumalı)
[ApiController]
[Route("api/workflow")]
[Authorize(Policy = "CsmsOnly")]
public sealed class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _wf;

    public WorkflowController(IWorkflowService wf) => _wf = wf;

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitInvoiceRequest req, CancellationToken ct)
    {
        // Türkçe: Korelasyon ID'sini header'dan alalım (middleware üretiyordu)
        var correlationId = HttpContext.TraceIdentifier;

        var (wf, snapshotJson) = await _wf.SubmitAsync(
            idemKey: req.IdempotencyKey,
            country: req.CountryCode,
            capability: req.Capability,
            preferKey: req.PreferProviderKey,
            amount: req.Amount,
            currency: req.Currency,
            description: req.Description,
            rawPayload: req.RawInvoicePayload,
            correlationId: correlationId,
            ct: ct
        );

        var resp = new SubmitInvoiceResponse
        {
            WorkflowId = wf.Id.ToString("N"),
            InvoiceId = wf.InvoiceId,
            Status = wf.Status,
            ProviderKey = wf.ProviderKey
        };
        return Accepted(resp); // 202 Accepted — kuyruğa alındı
    }

    [HttpGet("{workflowId}/status")]
    public async Task<IActionResult> Status([FromRoute] string workflowId, CancellationToken ct)
    {
        if (!Guid.TryParseExact(workflowId, "N", out var id)) return BadRequest("workflowId formatı N-GUID olmalıdır.");
        var wf = await _wf.GetStatusAsync(id, ct);
        if (wf is null) return NotFound();

        return Ok(new InvoiceStatusResponse
        {
            WorkflowId = wf.Id.ToString("N"),
            InvoiceId = wf.InvoiceId,
            Status = wf.Status,
            ProviderKey = wf.ProviderKey,
            CreatedAtUtc = wf.CreatedAtUtc,
            LastUpdatedUtc = wf.LastUpdatedUtc,
            LastError = wf.LastError
        });
    }
}
