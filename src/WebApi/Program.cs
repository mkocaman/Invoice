// Türkçe Açıklama:
// WebApi Program.cs - Serilog konfigürasyonu ile kategoriye göre farklı dosyalara yazdırma
// CorrelationMiddleware entegrasyonu

using Serilog;
using WebApi.Infrastructure.Logging;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Db;
using Infrastructure.Db.Services;
using Infrastructure.Providers.Config;
using Infrastructure.Providers.Core;
using Infrastructure.Providers.Contracts;
using Infrastructure.Providers.Adapters;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Türkçe: Correlation middleware'i en başa yakın konumda ekleyin
app.UseMiddleware<CorrelationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
