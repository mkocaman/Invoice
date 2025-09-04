// Türkçe Açıklama:
// İstenen sağlayıcı adına göre uygun IInvoiceProvider instance'ı verir.

using Infrastructure.Providers.Config;
using Infrastructure.Providers.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Core;

public interface IProviderFactory
{
    IInvoiceProvider Get(string name);
    IReadOnlyCollection<string> EnabledProviders { get; }
}

public sealed class ProviderFactory : IProviderFactory
{
    private readonly IServiceProvider _sp;
    private readonly ProviderOptions _opts;
    private readonly Dictionary<string, IInvoiceProvider> _cache = new(StringComparer.OrdinalIgnoreCase);

    public ProviderFactory(IServiceProvider sp, IOptions<ProviderOptions> opts)
    {
        _sp = sp;
        _opts = opts.Value;
    }

    public IReadOnlyCollection<string> EnabledProviders => _opts.Enabled;

    public IInvoiceProvider Get(string name)
    {
        if (_cache.TryGetValue(name, out var p)) return p;
        var impl = _sp.GetServices<IInvoiceProvider>().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (impl is null)
            throw new InvalidOperationException($"Sağlayıcı bulunamadı veya etkin değil: {name}");
        _cache[name] = impl;
        return impl;
    }
}
