using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class LucaAdapter : StubAdapterBase
{
    public override string Name => "LUCA";
    public LucaAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<LucaAdapter> l) : base(o, l) { }
}
