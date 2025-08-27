using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;

using Npgsql;
using Serilog;
using Serilog.Events;
using Invoice.Infrastructure.Data;
using Invoice.Infrastructure;
using Invoice.Infrastructure.Providers;
using Invoice.Infrastructure.Services;
using Invoice.Application.Interfaces;
using System.Globalization;
using System.Reflection;

// Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/invoice-api-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("Invoice API başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    // Türkçe kültür ayarlarını yapılandır
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR");

    // Serilog'u builder'a ekle
    builder.Host.UseSerilog();

    // Metrik sayaçları ekle
    var meter = new System.Diagnostics.Metrics.Meter("Invoice.Api");
    var wafBlockedCounter = meter.CreateCounter<long>("waf_blocked_total", "WAF tarafından bloklanan istekler");
    var rateLimitedCounter = meter.CreateCounter<long>("rate_limited_total", "Rate limit aşıldığında bloklanan istekler");

    // PostgreSQL bağlantı dizesini al
    var connectionString = builder.Configuration.GetConnectionString("Postgres") 
        ?? throw new InvalidOperationException("Postgres connection string bulunamadı");
    Log.Information("PostgreSQL bağlantı dizesi yapılandırıldı");

    // PostgreSQL veritabanı "yoksa yarat" mantığı
    // Bu kod uygulama başlatılırken çalışır ve veritabanının var olup olmadığını kontrol eder
    // Eğer veritabanı yoksa otomatik olarak oluşturur
    try
    {
        // Ana PostgreSQL sunucusuna bağlan (veritabanı belirtmeden)
        var masterConnectionString = connectionString.Replace("Database=invoice_db;", "Database=postgres;");
        
        using var connection = new NpgsqlConnection(masterConnectionString);
        await connection.OpenAsync();
        
        // invoice_db veritabanının var olup olmadığını kontrol et
        using var checkCommand = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = 'invoice_db'", connection);
        var databaseExists = await checkCommand.ExecuteScalarAsync();
        
        if (databaseExists == null)
        {
            Log.Information("invoice_db veritabanı bulunamadı, oluşturuluyor...");
            
            try
            {
                // Veritabanını oluştur - güvenli şekilde çift tırnakla kaçır
                using var createCommand = new NpgsqlCommand("CREATE DATABASE \"invoice_db\" WITH ENCODING 'UTF8'", connection);
                await createCommand.ExecuteNonQueryAsync();
                
                Log.Information("invoice_db veritabanı başarıyla oluşturuldu");
            }
            catch (Exception createEx)
            {
                Log.Warning(createEx, "Veritabanı oluşturma yetkisi yok veya başka bir hata oluştu. Uygulama devam ediyor...");
                // Uygulamayı durdurmuyoruz, sadece uyarı logluyoruz
            }
        }
        else
        {
            Log.Information("invoice_db veritabanı zaten mevcut");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Veritabanı oluşturma işlemi sırasında hata oluştu");
        throw;
    }

    // Entity Framework Core DbContext'i ekle - Migration assembly'yi belirt
    builder.Services.AddDbContext<InvoiceDbContext>(options =>
        options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Invoice.Infrastructure")));

    // Webhook imza doğrulama servisi ekle
    builder.Services.AddScoped<IWebhookSignatureValidator, WebhookSignatureValidator>();

    // Infrastructure servislerini ekle
    builder.Services.AddInfrastructure();
    
    // Provider Factory ve servisleri ekle
    builder.Services.AddScoped<IInvoiceProviderFactory, InvoiceProviderFactory>();
    builder.Services.AddScoped<UblBuilder>();
    builder.Services.AddScoped<PollyPolicyService>();
    
    // Tüm provider adapter'larını ekle
    builder.Services.AddTransient<ForibaProvider>();
    builder.Services.AddTransient<LogoProvider>();
    builder.Services.AddTransient<MikroProvider>();
    builder.Services.AddTransient<UyumsoftProvider>();
    builder.Services.AddTransient<KolayBiProvider>();
    builder.Services.AddTransient<ParasutProvider>();
    builder.Services.AddTransient<DiaProvider>();
    builder.Services.AddTransient<IdeaProvider>();
    builder.Services.AddTransient<BizimHesapProvider>();
    builder.Services.AddTransient<NetsisProvider>();
    
    // Mock signing service (dev ortamında)
    builder.Services.AddScoped<ISigningService, MockSigningService>();

    // Health Checks ekle
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<InvoiceDbContext>("Database")
        .AddCheck("Self", () => HealthCheckResult.Healthy("API çalışıyor"));

    // Rate limiting ekle
    builder.Services.AddRateLimiter(options =>
    {
        // Genel policy: IP başına 100 istek / 1 dk, burst 20
        options.AddFixedWindowLimiter("GeneralPolicy", limiterOptions =>
        {
            limiterOptions.PermitLimit = 100;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 20;
        });

        // Health check endpoint'leri için limitsiz
        options.AddNoLimiter("HealthPolicy");
    });

    // CORS politikası ekle
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Controllers ekle
    builder.Services.AddControllers();

    // OpenAPI/Swagger ekle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "Invoice API", 
            Version = "v1",
            Description = "EŞÜ ve şarj oturumu faturaları için API. SelfSign dev'de MockSigningService, ProviderSign dev'de mock çağrı kullanılır."
        });
        
        // XML dosyasından API dokümantasyonu oku
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    var app = builder.Build();

    // HTTP request pipeline'ını yapılandır
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    
    // Middleware sırası - Korelasyon ID log'larda görünsün diye en başta
    app.UseMiddleware<CorrelationMiddleware>();
    
    // İdempotency middleware'ini ekle (Correlation'dan hemen sonra)
    app.UseMiddleware<IdempotencyMiddleware>();
    
    // WAF middleware'ini ekle (güvenlik önce)
    app.UseMiddleware<WafMiddleware>();
    
    // Rate limiter'ı ekle
    app.UseRateLimiter();
    
    // Routing ekle
    app.UseRouting();

    // Health check endpoint'lerini ekle (rate limit yok)
    // /health - Liveness probe (uygulama çalışıyor mu?)
    app.MapHealthChecks("/health").RequireRateLimiting("HealthPolicy");

    // /health/ready - Readiness probe (uygulama istekleri kabul edebilir mi?)
    app.MapHealthChecks("/health/ready").RequireRateLimiting("HealthPolicy");

    // Entity Framework Core migration'larını uygula
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();
        await context.Database.MigrateAsync();
        Log.Information("Veritabanı migration'ları başarıyla uygulandı");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Veritabanı migration'ları uygulanırken hata oluştu");
        throw;
    }

    // API endpoint'lerini ekle (rate limit uygula)
    app.MapControllers().RequireRateLimiting("GeneralPolicy");

    Log.Information("Invoice API başlatıldı ve hazır");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Invoice API başlatılırken beklenmeyen hata oluştu");
}
finally
{
    Log.CloseAndFlush();
}
