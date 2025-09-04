using Invoice.Application.Interfaces;
using Infrastructure.Providers;
using Infrastructure.Providers.Health;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Providers;

// Türkçe: Sağlık gözlemi uçları
[ApiController]
[Route("api/providers/health")]
[Authorize(Policy = "CsmsOnly")]
public sealed class ProviderHealthController : ControllerBase
{
    private readonly IProviderRegistry _registry;
    private readonly IProviderHealthService _health;

    public ProviderHealthController(IProviderRegistry registry, IProviderHealthService health)
    {
        _registry = registry;
        _health = health;
    }

    [HttpGet("{countryCode}")]
    public async Task<IActionResult> Snapshot([FromRoute] string countryCode, CancellationToken ct)
    {
        // Türkçe: Basit snapshot — her sağlayıcının health'i
        var list = _registry.GetByCountry(countryCode);
        var items = new List<object>();
        foreach (var p in list)
        {
            var ok = await _health.IsHealthyAsync(p, ct);
            items.Add(new { ProviderType = p.ProviderType.ToString(), healthy = ok });
        }
        return Ok(new { countryCode, items });
    }
}
