using Effanville.TradingStructures.Common.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Exchanges.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExchangeServices(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IExchangeSessionService, ExchangeSessionService>()
            .AddSingleton<IService>(a => a.GetService<IExchangeSessionService>());
    }
}
