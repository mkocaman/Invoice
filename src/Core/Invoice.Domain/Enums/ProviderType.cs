namespace Invoice.Domain.Enums
{
    // Türkçe: Ülke-agnostik enum; değerler ülkelere yayılabilir, UI/Config ile filtrelenecek.
    public enum ProviderType
    {
        // TR (Türkiye)
        FORIBA = 1,
        LOGO = 2,
        MIKRO = 3,
        UYUMSOFT = 4,
        KOLAYBI = 5,
        PARASUT = 6,
        DIA = 7,          // DİA
        IDEA = 8,         // İdea
        BIZIMHESAP = 9,   // BizimHesap
        NETSIS = 10,      // Logo / Netsis
        
        // UZ (Özbekistan)
        FAKTURA_UZ = 100,
        DIDOX_UZ = 101,
        
        // KZ (Kazakistan)
        IS_ESF_KZ = 200
    }
}
