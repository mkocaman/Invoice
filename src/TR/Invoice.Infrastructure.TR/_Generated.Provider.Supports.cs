// Türkçe yorum: Bu dosya, TR sağlayıcılarının eksik IInvoiceProvider üyelerini "partial" ile tek noktadan ekler.

using System;
using System.Collections.Generic;
using Invoice.Application.Interfaces;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.TR.Providers
{
    // Türkçe yorum: Ortak yardımcı - TR destekli sağlayıcılar listesi
    internal static class ProviderSupport
    {
        // Türkçe yorum: TR odaklı sağlayıcılar.
        public static bool SupportsCountry(string countryCode)
            => string.Equals(countryCode, "TR", StringComparison.OrdinalIgnoreCase);
    }
}

// DIA
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class DiaProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// FORIBA
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class ForibaProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// IDEA
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class IdeaProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// LOGO
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class LogoProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// KOLAYBİ
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class KolayBiProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// MIKRO
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class MikroProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// NETSIS
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class NetsisProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// PARASUT
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class ParasutProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// BIZIMHESAP
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class BizimHesapProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}

// UYUMSOFT
namespace Invoice.Infrastructure.TR.Providers
{
    public partial class UyumsoftProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType;
    }
}
