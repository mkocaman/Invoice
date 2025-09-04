using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class MikroAdapter : StubAdapterBase
{
    public override string Name => "MIKRO";
    public MikroAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<MikroAdapter> l) : base(o, l) { }
}
