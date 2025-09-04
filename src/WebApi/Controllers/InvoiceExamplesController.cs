// Türkçe: Örnek InvoicesController — CsmsOnly ile korunur, sayfalama ve doğrulama gösterir.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Invoices;
using WebApi.Infrastructure.Pagination;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/invoice-examples")]
    [Authorize(Policy = "CsmsOnly")] // Türkçe: Phase 1'de tanımlanan politika
    public sealed class InvoiceExamplesController : ControllerBase
    {
        // TODO: Gerçek veri erişimi (EF Core/Repository) enjekte edilecek.
        // Bu örnekte in-memory sahte veri ile çalışıyoruz.

        // GET api/invoice-examples?page=1&pageSize=20&sort=createdAt desc&filter=status=Sent
        [HttpGet]
        public IActionResult GetList([FromQuery] InvoiceQueryFilter q)
        {
            // [SIMULATION] Türkçe: Örnek veri; gerçek hayatta DB'den çekilir ve filtrelenir/sıralanır.
            var total = 125; // örnek toplam
            var items = Enumerable.Range(1, q.PageSize)
                                  .Select(i => new { InvoiceNo = $"INV-{(q.Page-1)*q.PageSize + i:000000}", Amount = 100 + i, Currency = "TRY" })
                                  .ToList();

            // Türkçe: Header'lara meta bilgi yaz
            HttpContext.WritePaginationHeaders(q.Page, q.PageSize, total);

            // Türkçe: Body zarfı
            var resp = new PagedResponse<object>(items, q.Page, q.PageSize, total);
            return Ok(resp);
        }

        // POST api/invoice-examples
        [HttpPost]
        public IActionResult Create([FromBody] CreateInvoiceRequest request)
        {
            // Türkçe: Burada servis çağrısı ile fatura oluşturulur.
            var createdId = Guid.NewGuid().ToString("N");
            return CreatedAtAction(nameof(GetById), new { id = createdId }, new { id = createdId, status = "Created" });
        }

        // GET api/invoice-examples/{id}
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] string id)
        {
            // [SIMULATION] Türkçe: Örnek dönüş
            return Ok(new { id, status = "Created", amount = 123.45m, currency = "TRY" });
        }
    }
}
