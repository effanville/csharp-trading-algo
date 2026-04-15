using Effanville.TradingStructures.Common.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.OrderManagement.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderManagement(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<IOrderManagementService, OrderManagementService>()
            .AddSingleton<IService>(a => a.GetRequiredService<IOrderManagementService>());
    }

}
