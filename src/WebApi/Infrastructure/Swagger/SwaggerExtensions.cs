// Türkçe: Swagger & OpenAPI yapılandırması - Bearer/JWT auth desteği içerir.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WebApi.Infrastructure.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerAndOpenApi(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Invoice API",
                    Version = "v1",
                    Description = "CSMS tarafından yetkilendirilen e-Fatura servisleri"
                });

                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "CSMS tarafından imzalanan JWT'yi 'Bearer {token}' formatında gönderin."
                };

                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { scheme, new List<string>() }
                });
            });

            return services;
        }
    }
}
