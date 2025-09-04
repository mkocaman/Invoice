using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class KolayBiAdapter : StubAdapterBase
{
    public override string Name => "KOLAYBI";
    public KolayBiAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<KolayBiAdapter> l) : base(o, l) { }
}
