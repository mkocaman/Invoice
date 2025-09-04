using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class UyumsoftAdapter : StubAdapterBase
{
    public override string Name => "UYUMSOFT";
    public UyumsoftAdapter(IOptions<Infrastructure.Providers.Config.ProviderOptions> o, ILogger<UyumsoftAdapter> l) : base(o, l) { }
}
