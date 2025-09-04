// Türkçe: Sayfalı sonuçlar için basit zarf (response body).
namespace WebApi.Infrastructure.Pagination
{
    public sealed class PagedResponse<T>
    {
        // Türkçe: Dönen kayıtlar
        public IReadOnlyList<T> Data { get; init; }

        // Türkçe: Mevcut sayfa (1-based)
        public int Page { get; init; }

        // Türkçe: Sayfa boyutu
        public int PageSize { get; init; }

        // Türkçe: Toplam kayıt sayısı
        public long TotalCount { get; init; }

        public PagedResponse(IReadOnlyList<T> data, int page, int pageSize, long totalCount)
        {
            Data = data;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
