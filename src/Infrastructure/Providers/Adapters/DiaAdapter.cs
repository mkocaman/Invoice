using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class DiaAdapter : StubAdapterBase
{
    public override string Name => "DIA";
    public DiaAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<DiaAdapter> l) : base(o, l) { }
}
