// Türkçe: ModelState (FluentValidation) hatalarını RFC7807 ProblemDetails formatında döndürmek için özel fabrika.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace WebApi.Infrastructure.ProblemDetails
{
    public sealed class ValidationProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ILogger<ValidationProblemDetailsFactory> _logger;

        public ValidationProblemDetailsFactory(ILogger<ValidationProblemDetailsFactory> logger)
        {
            _logger = logger;
        }

        public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null,
            string? title = null, string? type = null, string? detail = null, string? instance = null)
        {
            var problem = new ProblemDetails
            {
                Status = statusCode ?? StatusCodes.Status500InternalServerError,
                Title = title ?? "İşlenemeyen hata",
                Type = type ?? "about:blank",
                Detail = detail,
                Instance = instance ?? httpContext.Request.Path
            };
            return problem;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext,
            ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null,
            string? type = null, string? detail = null, string? instance = null)
        {
            if (modelStateDictionary == null)
                throw new ArgumentNullException(nameof(modelStateDictionary));

            var problem = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = statusCode ?? StatusCodes.Status400BadRequest,
                Title = title ?? "Geçersiz istek",
                Type = type ?? "https://httpstatuses.com/400",
                Detail = detail,
                Instance = instance ?? httpContext.Request.Path
            };

            // Türkçe: İzleme için TraceId ekleyelim
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            // Türkçe: Kısa bir log düşelim (kapsamlı loglar Serilog ile dosyaya gider)
            _logger.LogWarning("Doğrulama hatası: {Path}, TraceId: {TraceId}", httpContext.Request.Path, httpContext.TraceIdentifier);

            return problem;
        }
    }
}
