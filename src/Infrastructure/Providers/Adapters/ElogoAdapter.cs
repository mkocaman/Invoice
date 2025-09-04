using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class ElogoAdapter : StubAdapterBase
{
    public override string Name => "ELOGO";
    public ElogoAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<ElogoAdapter> l) : base(o, l) { }
}
