// [KILAVUZ] TR Kimlik Doğrulama — VKN(10) / TCKN(11) hızlı kontrol
namespace Invoice.Application.Helpers
{
    public static class TurkishIdHelper
    {
        public static bool IsDigits(string? s) => !string.IsNullOrWhiteSpace(s) && s.All(char.IsDigit);
        public static bool IsVkn(string? s) => IsDigits(s) && s!.Length == 10;
        public static bool IsTckn(string? s) => IsDigits(s) && s!.Length == 11;
    }
}
