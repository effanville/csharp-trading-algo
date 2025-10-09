using Effanville.TradingStructures.Common.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.StaticData.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStaticDataServices(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IStaticDataService, StaticDataService>()
            .AddSingleton<IService>(a => a.GetService<IStaticDataService>());
    }
}