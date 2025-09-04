using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class SovosAdapter : StubAdapterBase
{
    public override string Name => "SOVOS";
    public SovosAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<SovosAdapter> l) : base(o, l) { }
}
