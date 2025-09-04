// Türkçe Açıklama:
// WebApi Program.cs - Serilog konfigürasyonu ile kategoriye göre farklı dosyalara yazdırma
// CorrelationMiddleware entegrasyonu
// PHASE 1 eklemeleri (CSMS-JWT, CORS, RateLimit, Swagger, Exception)

using Serilog;
using WebApi.Infrastructure.Logging;
using WebApi.Infrastructure.Middleware;
using WebApi.Infrastructure.RateLimiting;
using WebApi.Infrastructure.Security.Jwt;
using WebApi.Infrastructure.Security.Policies;
using WebApi.Infrastructure.Swagger;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Db;
using Infrastructure.Db.Services;
using Infrastructure.Providers.Config;
using Infrastructure.Providers.Core;
using Infrastructure.Providers.Contracts;
using Infrastructure.Providers.Adapters;

var builder = WebApplication.CreateBuilder(args);

// [GÜVENLİK] Kestrel max request boyutu (örn. 25MB) — XML gövdeleri için koruma
builder.WebHost.ConfigureKestrel(k =>
{
    // Türkçe: Büyük istekleri sınırlayalım
    k.Limits.MaxRequestBodySize = 25 * 1024 * 1024; // 25 MB
});

// Serilog konfigürasyonu - kategoriye göre farklı dosyalara yazdırma
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // Uygulama logları
    .WriteTo.File("logs/app/app-.log", rollingInterval: RollingInterval.Day)
    // DB kategorisi
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties.ContainsKey("SourceContext") && 
            le.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore"))
        .WriteTo.File("logs/db/db-.log", rollingInterval: RollingInterval.Day))
    // RabbitMQ kategorisi
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties.ContainsKey("SourceContext") && 
            le.Properties["SourceContext"].ToString().Contains("RabbitMq"))
        .WriteTo.File("logs/rabbitmq/rabbitmq-.log", rollingInterval: RollingInterval.Day))
    .CreateLogger();

builder.Host.UseSerilog();

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

// Add services to the container.
builder.Services.AddControllers();

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

// [EXCEPTION] global
app.UseGlobalExceptionHandling();

// [SECURE] HSTS/HTTPS redirect üretimde önerilir (gerekirse açın)
// if (!app.Environment.IsDevelopment()) { app.UseHsts(); app.UseHttpsRedirection(); }

app.UseCors("CorsPolicy");

app.UseRateLimiter();

// Türkçe: Correlation middleware'i en başa yakın konumda ekleyin
app.UseMiddleware<CorrelationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// [SWAGGER] sadece dev'de değil, iç ağda prod'da da açık tutabilirsiniz (IP kısıtı önerilir)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
    c.DisplayRequestDuration();
});

app.MapControllers();
// Not: CsmsOnly politikası sadece gerekli controller'larda [Authorize(Policy = "CsmsOnly")] ile uygulanır.
// TestController gibi AllowAnonymous controller'lar bu politikadan etkilenmez.

app.Run();