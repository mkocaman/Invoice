// Türkçe Açıklama:
// WebApi Program.cs - Serilog konfigürasyonu ile kategoriye göre farklı dosyalara yazdırma
// CorrelationMiddleware entegrasyonu
// PHASE 1 eklemeleri (CSMS-JWT, CORS, RateLimit, Swagger, Exception)
// PHASE 2 eklemeleri (FluentValidation, Pagination, ProblemDetails)
// PHASE 3 eklemeleri (Serilog rolling files, Health checks, HTTP resilience)

using Serilog;
using WebApi.Infrastructure.Logging;
using WebApi.Infrastructure.Middleware;
using WebApi.Infrastructure.RateLimiting;
using WebApi.Infrastructure.Security.Jwt;
using WebApi.Infrastructure.Security.Policies;
using WebApi.Infrastructure.Swagger;
using WebApi.Infrastructure.ProblemDetails;
using WebApi.Infrastructure.Health;
using WebApi.Infrastructure.Http.Resilience;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Db;
using Infrastructure.Db.Services;
using Infrastructure.Providers.Config;
using Infrastructure.Providers.Core;
using Infrastructure.Providers.Contracts;
using Infrastructure.Providers.Adapters;

// Türkçe: Serilog'u appsettings'ten kur (dosya/konsol)
SerilogExtensions.ConfigureSerilogFromConfiguration(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build());

var builder = WebApplication.CreateBuilder(args);

// Türkçe: ASP.NET loglarını Serilog'a yönlendir
builder.Host.UseSerilog();

// [GÜVENLİK] Kestrel max request boyutu (örn. 25MB) — XML gövdeleri için koruma
builder.WebHost.ConfigureKestrel(k =>
{
    // Türkçe: Büyük istekleri sınırlayalım
    k.Limits.MaxRequestBodySize = 25 * 1024 * 1024; // 25 MB
});

// [CORS] Basit whitelist
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", p =>
    {
        p.WithOrigins(allowedOrigins)
         .AllowAnyMethod()
         .AllowAnyHeader();
        // Türkçe: Eğer cookie/credential gerekiyorsa .AllowCredentials() eklenebilir.
    });
});

// [AUTH] CSMS JWT doğrulama + [AUTHZ] CsmsOnly politikası
builder.Services.AddCsmsJwtAuthentication(builder.Configuration);
builder.Services.AddCsmsAuthorization();

// [RATELIMIT]
builder.Services.AddBasicRateLimiting(builder.Configuration);

// [EXCEPTION] Global exception handling
builder.Services.AddGlobalExceptionHandling();

// [SWAGGER]
builder.Services.AddSwaggerAndOpenApi();

// [VALIDATION] FluentValidation — otomatik model doğrulama (400)
builder.Services
    .AddFluentValidationAutoValidation()
    .AddValidatorsFromAssemblyContaining<WebApi.Infrastructure.Validation.CreateInvoiceRequestValidator>();

// [PROBLEMDETAILS] ModelState hatalarını ProblemDetails'a dönüştürmek için özel fabrika
builder.Services.AddSingleton<Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory, ValidationProblemDetailsFactory>();

// [HEALTH] Liveness & Readiness
builder.Services.AddInvoiceHealthChecks(builder.Configuration);

// [HTTP RESILIENCE] Not: Kayıtlı named HttpClient'larınıza bu politikayı ekleyin.
// Örnek (genel amaçlı):
builder.Services.AddHttpClient("provider-default")
    .AddResiliencePolicies();

// [CONTROLLERS] Controller davranışları
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Türkçe: ModelState hataları otomatik 400 döndürsün (ProblemDetailsFactory devrede)
        options.SuppressModelStateInvalidFilter = false;
    });

// DbContext ekleme
builder.Services.AddDbContext<AppDbContext>(options =>
{
    DbContextFactory.Configure(
        options,
        LoggerFactory.Create(lb => lb.AddSerilog()), 
        builder.Configuration,
        builder.Environment.IsDevelopment()
    );
});

// Audit servisi
builder.Services.AddScoped<IInvoiceAuditService, InvoiceAuditService>();

// Türkçe: ProviderOptions binding
builder.Services.Configure<ProviderOptions>(builder.Configuration.GetSection("Providers"));
// Türkçe: Factory
builder.Services.AddSingleton<IProviderFactory, ProviderFactory>();

// Türkçe: Foriba gerçek adapter (HttpClient ile)
builder.Services.AddHttpClient<ForibaAdapter>();
builder.Services.AddTransient<IInvoiceProvider, ForibaAdapter>();

// Türkçe: 9 stub adapter
builder.Services.AddTransient<IInvoiceProvider, LogoAdapter>();
builder.Services.AddTransient<IInvoiceProvider, KolayBiAdapter>();
builder.Services.AddTransient<IInvoiceProvider, MikroAdapter>();
builder.Services.AddTransient<IInvoiceProvider, UyumsoftAdapter>();
builder.Services.AddTransient<IInvoiceProvider, ElogoAdapter>();
builder.Services.AddTransient<IInvoiceProvider, ParasutAdapter>();
builder.Services.AddTransient<IInvoiceProvider, SovosAdapter>();
builder.Services.AddTransient<IInvoiceProvider, DiaAdapter>();
builder.Services.AddTransient<IInvoiceProvider, LucaAdapter>();

// RabbitMQ servisleri
builder.Services.AddSingleton<IConnectionFactory>(provider =>
{
    var factory = new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest"
    };
    return factory;
});

var app = builder.Build();

// [SERILOG] İstek loglama (otomatik — status/süre/route)
app.UseSerilogRequestLogging();

// [EXCEPTION] global
app.UseGlobalExceptionHandling();

// [SECURE] HSTS/HTTPS redirect üretimde önerilir (gerekirse açın)
// if (!app.Environment.IsDevelopment()) { app.UseHsts(); app.UseHttpsRedirection(); }

app.UseCors("CorsPolicy");

app.UseRateLimiter();

// Türkçe: Correlation middleware'i en başa yakın konumda ekleyin
app.UseMiddleware<CorrelationMiddleware>();

// [HEALTH ENDPOINTS] - Map before authorization to ensure they're anonymous
// Türkçe: Liveness (anonim erişime açık olabilir)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { status = report.Status.ToString(), checks = report.Entries.Keys });
        await ctx.Response.WriteAsync(body);
    }
});

// Türkçe: Readiness (genelde iç ağdan; dilersen CsmsOnly ile koru)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var details = report.Entries.ToDictionary(
            e => e.Key,
            e => new { status = e.Value.Status.ToString(), description = e.Value.Description }
        );
        var body = JsonSerializer.Serialize(new { status = report.Status.ToString(), details });
        await ctx.Response.WriteAsync(body);
    }
});

app.UseAuthentication();
app.UseAuthorization();

// [SWAGGER] sadece dev'de değil, iç ağda prod'da da açık tutabilirsiniz (IP kısıtı önerilir)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
    c.DisplayRequestDuration();
});

// Türkçe: Controller'lar. Not: InvoicesController vb. üzerinde [Authorize(Policy="CsmsOnly")] kullanın.
app.MapControllers();

app.Run();

// Make Program class accessible for testing
public partial class Program { }