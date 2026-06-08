using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.StockMarket.Implementation;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.StockMarket.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimulationExchange(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(a => StockMarketAdapterSettings.Default())
            .AddSingleton<IStockMarketAdapter, SimulationExchange>()
            .AddSingleton<IService>(x => x.GetService<IStockMarketAdapter>()!);
    }
}
