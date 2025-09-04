// Türkçe: Workflow servisleri DI kayıtları
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Workflows
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInvoiceWorkflow(this IServiceCollection services)
        {
            services.AddScoped<IWorkflowService, WorkflowService>();
            return services;
        }
    }
}
