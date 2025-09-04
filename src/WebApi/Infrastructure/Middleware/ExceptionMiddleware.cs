// Türkçe: Global istisna yakalayıcı - RFC7807 ProblemDetails çıktısı üretir.
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebApi.Infrastructure.Middleware
{
    // Türkçe: Global exception'ları yakalayarak standart ProblemDetails döndürür.
    public sealed class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Türkçe: Burada hata logu (structured) atılır
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu. Path: {Path}, TraceId: {TraceId}",
                    context.Request.Path, context.TraceIdentifier);

                var problem = new
                {
                    type = "https://httpstatuses.com/500",
                    title = "Beklenmeyen hata",
                    status = (int)HttpStatusCode.InternalServerError,
                    traceId = context.TraceIdentifier
                };

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
        {
            // Türkçe: DI konteynerına middleware eklenir
            services.AddTransient<ExceptionMiddleware>();
            return services;
        }

        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            // Türkçe: Pipeline'a global middleware eklenir
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
