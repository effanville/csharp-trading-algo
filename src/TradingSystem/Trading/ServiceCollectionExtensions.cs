using Effanville.TradingStructures.Common.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingSystem.Trading;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderManagement(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IOrderListener, OrderListener>()
            .AddSingleton<IService>(a => a.GetService<IOrderListener>()!);
    }
}
