using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessingOrders.Infrastructure.Kafka;
using ProcessingOrders.Infrastructure.Services.Promotions;

namespace ProcessingOrders.Infrastructure.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<PromotionsGrpcSettings>(
            configuration.GetSection("PromotionsGrpcService"));
        
        services.AddScoped<IPromotionsService, PromotionsService>();
        
        services.Configure<KafkaSettings>(
            configuration.GetSection("KafkaSettings"));
        
        services.AddSingleton<IMessageBus, KafkaMessageBus>();

        return services;
    }
} 