using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Workflows.Background
{
    public static class ServiceCollectionExtensionsBackground
    {
        public static IServiceCollection AddRabbitAdapters(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitPublisher, RabbitPublisherAdapter>();
            services.AddSingleton<IMessageConsumer, MessageConsumerAdapter>();
            return services;
        }
    }
}
