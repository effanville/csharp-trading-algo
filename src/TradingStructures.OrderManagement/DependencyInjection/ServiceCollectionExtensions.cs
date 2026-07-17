using Effanville.TradingStructures.Common.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Effanville.TradingStructures.OrderManagement.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderManagement(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddOptions<OrderManagementSettings>();
        return serviceCollection
            .AddSingleton(sp => sp.GetRequiredService<IOptions<OrderManagementSettings>>().Value)
            .AddSingleton<IOrderManagementService, OrderManagementService>()
            .AddSingleton<IService>(a => a.GetRequiredService<IOrderManagementService>());
    }

}
