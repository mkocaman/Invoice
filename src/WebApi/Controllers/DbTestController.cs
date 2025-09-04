using Infrastructure.Db.Entities;
using Infrastructure.Db.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/dbtest")]
public class DbTestController : ControllerBase
{
    private readonly IInvoiceAuditService _audit;

    public DbTestController(IInvoiceAuditService audit)
    {
        _audit = audit;
    }

    // ✅ Önceki audit-sample endpoint duruyor
    [HttpPost("audit-sample")]
    public async Task<IActionResult> AuditSample([FromBody] dynamic? input)
    {
        var audit = new InvoiceAudit
        {
            InvoiceId = Guid.NewGuid().ToString("n"),
            ExternalInvoiceNumber = "INV-TEST-001",
            EventType = "SENT",
            StatusFrom = "SIGNED",
            StatusTo = "SENT",
            SystemCode = "KZ",
            XmlPayload = "<Invoice><Number>INV-TEST-001</Number></Invoice>",
            JsonPayload = "{\"number\":\"INV-TEST-001\",\"total\":123.45}",
            RequestBody = "{\"token\":\"SECRET\",\"data\":\"...\"}",
            ResponseBody = "{\"status\":\"OK\",\"id\":\"abc\"}",
            Simulation = true,
            Actor = "api:test",
            Notes = "Insert test sample"
        };

        var id = await _audit.LogAsync(audit);
        return Ok(new { message = "Audit kaydı eklendi", id, audit.InvoiceId });
    }

    // ✅ Yeni endpoint: DB'ye basit insert testi
    [HttpPost("audit-insert")]
    public async Task<IActionResult> AuditInsert()
    {
        var audit = new InvoiceAudit
        {
            InvoiceId = Guid.NewGuid().ToString("n"),
            EventType = "CREATED",
            StatusFrom = null,
            StatusTo = "CREATED",
            SystemCode = "UZ",
            XmlPayload = "<Invoice><Number>INV-INSERT-001</Number></Invoice>",
            JsonPayload = "{\"number\":\"INV-INSERT-001\",\"total\":99.99}",
            Simulation = true,
            Actor = "api:insert-test",
            Notes = "Manual insert test"
        };

        var id = await _audit.LogAsync(audit);
        return Ok(new { message = "Audit insert başarılı", id, audit.InvoiceId });
    }
}