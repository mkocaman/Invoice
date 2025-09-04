using Invoice.Application.Interfaces;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Providers;

// Türkçe: Sağlayıcı teşhis/yönetim uçları (CSMS korumasıyla)
[ApiController]
[Route("api/providers")]
[Authorize(Policy = "CsmsOnly")]
public sealed class ProvidersController : ControllerBase
{
    private readonly IProviderRegistry _registry;
    private readonly MultiProviderOptions _opts;

    public ProvidersController(IProviderRegistry registry, Microsoft.Extensions.Options.IOptions<MultiProviderOptions> opts)
    {
        _registry = registry;
        _opts = opts.Value;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        // Türkçe: Sağlayıcı listesi (ülke/kilit alanlar)
        var items = _registry.GetAll().Select(p => new { ProviderType = p.ProviderType.ToString() }).ToList();
        return Ok(new { defaultCountry = _opts.DefaultCountry, count = items.Count, providers = items });
    }

    [HttpGet("{countryCode}")]
    public IActionResult GetByCountry([FromRoute] string countryCode)
    {
        var items = _registry.GetByCountry(countryCode).Select(p => new { ProviderType = p.ProviderType.ToString() }).ToList();
        return Ok(new { countryCode, count = items.Count, providers = items });
    }

    [HttpGet("{countryCode}/resolve")]
    public IActionResult Resolve([FromRoute] string countryCode, [FromQuery] string capability = "eInvoice", [FromQuery] string? preferKey = null)
    {
        var p = _registry.ResolveBest(countryCode, capability, preferKey);
        if (p == null) return NotFound(new { message = "Uygun sağlayıcı bulunamadı." });
        return Ok(new { ProviderType = p.ProviderType.ToString() });
    }
}
