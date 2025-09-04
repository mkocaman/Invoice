// Türkçe: HTTP header'larına sayfalama meta bilgilerini eklemek için yardımcılar.
// Link header örneği: <https://api/foo?page=2&pageSize=20>; rel="next"
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace WebApi.Infrastructure.Pagination
{
    public static class PaginationHttpExtensions
    {
        // Türkçe: Response header'a X-Total-Count ve Link ekler.
        public static void WritePaginationHeaders(this HttpContext http, int page, int pageSize, long totalCount)
        {
            http.Response.Headers["X-Total-Count"] = totalCount.ToString();

            var request = http.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}";

            string BuildLink(int p, string rel)
            {
                var dict = new Dictionary<string, string?>()
                {
                    ["page"] = p.ToString(),
                    ["pageSize"] = pageSize.ToString()
                };
                var url = QueryHelpers.AddQueryString(baseUrl, dict!);
                return $"<{url}>; rel=\"{rel}\"";
            }

            var links = new List<string>();
            if (page > 1) links.Add(BuildLink(page - 1, "prev"));

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (page < totalPages) links.Add(BuildLink(page + 1, "next"));

            if (links.Count > 0)
                http.Response.Headers["Link"] = string.Join(", ", links);
        }
    }
}
