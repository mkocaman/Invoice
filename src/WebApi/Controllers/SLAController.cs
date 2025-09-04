using Infrastructure.Db.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

// Türkçe: SLA raporlarını API üzerinden sunar
[ApiController]
[Route("api/sla")]
public class SLAController : ControllerBase
{
    private readonly SLAReportService _sla;

    public SLAController(SLAReportService sla) => _sla = sla;

    [HttpGet("{systemCode}/latency")]
    public async Task<IActionResult> GetLatency(string systemCode)
    {
        var ms = await _sla.GetAvgLatencyMsAsync(systemCode);
        return Ok(new { systemCode, avgLatencyMs = ms });
    }

    [HttpGet("{systemCode}/error-rate")]
    public async Task<IActionResult> GetErrorRate(string systemCode)
    {
        var rate = await _sla.GetErrorRateAsync(systemCode);
        return Ok(new { systemCode, errorRate = rate });
    }
}
