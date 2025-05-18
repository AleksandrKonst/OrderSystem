using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessingOrders.Application.Services;
using ProcessingOrders.CoreDomain.Common;
using ProcessingOrders.Infrastructure.Configuration;

namespace ProcessingOrders.Application.Configuration;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddScoped<DiscountService>();
        
        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddInfrastructureServices(configuration);

        return services;
    }
} 