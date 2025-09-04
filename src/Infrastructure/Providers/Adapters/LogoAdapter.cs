using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class LogoAdapter : StubAdapterBase
{
    public override string Name => "LOGO";
    public LogoAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<LogoAdapter> l) : base(o, l) { }
}
