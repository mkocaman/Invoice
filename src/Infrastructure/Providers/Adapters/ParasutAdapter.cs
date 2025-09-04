using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class ParasutAdapter : StubAdapterBase
{
    public override string Name => "PARASUT";
    public ParasutAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<ParasutAdapter> l) : base(o, l) { }
}
