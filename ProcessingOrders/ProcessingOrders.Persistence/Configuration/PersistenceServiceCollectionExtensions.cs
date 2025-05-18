using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessingOrders.CoreDomain.Repositories;
using ProcessingOrders.Persistence.Context;
using ProcessingOrders.Persistence.Repositories;
using System;

namespace ProcessingOrders.Persistence.Configuration;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("OrdersDb"),
                npgsqlOptions => 
                {
                    npgsqlOptions.EnableRetryOnFailure(3);
                }));
        
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
} 