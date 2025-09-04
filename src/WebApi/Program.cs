using WebApi.Infrastructure.Middleware;
using WebApi.Infrastructure.RateLimiting;
using WebApi.Infrastructure.Security.Jwt;
using WebApi.Infrastructure.Security.Policies;
using WebApi.Infrastructure.Swagger;
using WebApi.Infrastructure.ProblemDetails;
using WebApi.Infrastructure.Health;
using WebApi.Infrastructure.Logging;
using WebApi.Infrastructure.Http.Resilience;
using Infrastructure.Options;
using WebApi.Infrastructure.StartupValidation;
using Infrastructure.Providers;
using Infrastructure.Providers.Http;
using Infrastructure.Providers.Adapters.TR;
using Infrastructure.Workflows;
using Infrastructure.Workflows.Background;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

SerilogExtensions.ConfigureSerilogFromConfiguration(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build());

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// CORS/Security/RateLimit/Swagger/Validation/ProblemDetails/Health/HttpClient Resilience
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", p => p.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>()).AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddCsmsJwtAuthentication(builder.Configuration);
builder.Services.AddCsmsAuthorization();
builder.Services.AddBasicRateLimiting(builder.Configuration);
builder.Services.AddGlobalExceptionHandling();
builder.Services.AddSwaggerAndOpenApi();
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<WebApi.Infrastructure.Validation.CreateInvoiceRequestValidator>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory, ValidationProblemDetailsFactory>();
builder.Services.AddInvoiceHealthChecks(builder.Configuration);
builder.Services.AddHttpClient("provider-default").AddResiliencePolicies();

// PHASE 6: Multi-provider core
builder.Services.AddMultiProviderCore();

// PHASE 7: Workflow
builder.Services.AddInvoiceWorkflow();
builder.Services.AddRabbitAdapters();

// PHASE 9: Provider secrets + typed clients + adapters
builder.Services.AddOptions<ProviderSecretOptions>().BindConfiguration("ProviderSecrets").ValidateDataAnnotations();
builder.Services.AddHttpClient<IForibaClient, ForibaClient>();
builder.Services.AddHttpClient<ILogoClient, LogoClient>();

builder.Services.AddScoped<Invoice.Application.Providers.IProviderAdapter, ForibaAdapter>();
builder.Services.AddScoped<Invoice.Application.Providers.IProviderAdapter, LogoAdapter>();

// Startup validation (kritik sır kontrolü) — sadece Production|Staging'da zorunlu kılalım
if (!builder.Environment.IsDevelopment())
{
    var tmp = builder.Configuration.GetSection("ProviderSecrets").Get<ProviderSecretOptions>() ?? new();
    StartupValidators.ValidateProviderSecrets(tmp);
}

builder.Services.AddControllers().ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = false);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseGlobalExceptionHandling();
app.UseCors("CorsPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1"); c.DisplayRequestDuration(); });
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
// app.MapMetrics("/metrics");
app.MapControllers();
app.Run();