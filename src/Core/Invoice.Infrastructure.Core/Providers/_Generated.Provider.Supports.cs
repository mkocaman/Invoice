// Türkçe yorum: Bu dosya, sağlayıcıların eksik IInvoiceProvider üyelerini "partial" ile tek noktadan ekler.
// Not: Gerektiğinde ülke listesi genişletilebilir.

using System;
using System.Collections.Generic;
using Invoice.Application.Interfaces;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.Providers
{
    // Core Infrastructure Provider'ları için Supports/SupportsCountry implementasyonları
    // Bu dosya otomatik olarak oluşturulmuştur - manuel düzenlemeyin
    
    // BIZIMHESAP
    public partial class BizimHesapProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.BIZIMHESAP;
    }
    
    // DIA
    public partial class DiaProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.DIA;
    }
    
    // FORIBA
    public partial class ForibaProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.FORIBA;
    }
    
    // IDEA
    public partial class IdeaProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.IDEA;
    }
    
    // KOLAYBI
    public partial class KolayBiProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.KOLAYBI;
    }
    
    // LOGO
    public partial class LogoProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.LOGO;
    }
    
    // MIKRO
    public partial class MikroProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.MIKRO;
    }
    
    // NETSIS
    public partial class NetsisProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.NETSIS;
    }
    
    // PARASUT
    public partial class ParasutProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.PARASUT;
    }
    
    // UYUMSOFT
    public partial class UyumsoftProvider
    {
        public bool SupportsCountry(string countryCode) => ProviderSupport.SupportsCountry(countryCode);
        public bool Supports(ProviderType type) => type == ProviderType.UYUMSOFT;
    }
}

// ProviderSupport yardımcı sınıfı
namespace Invoice.Infrastructure.Providers
{
    internal static class ProviderSupport
    {
        public static bool SupportsCountry(string countryCode) => 
            string.Equals(countryCode, "TR", StringComparison.OrdinalIgnoreCase);
    }
}
