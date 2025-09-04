using Infrastructure.Providers.Core;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/providers")]
public class ProvidersController : ControllerBase
{
    private readonly IProviderFactory _factory;

    public ProvidersController(IProviderFactory factory)
    {
        _factory = factory;
    }

    [HttpGet]
    public IActionResult GetProviders()
    {
        return Ok(new { 
            message = "Etkin sağlayıcılar", 
            providers = _factory.EnabledProviders.ToArray() 
        });
    }
}
