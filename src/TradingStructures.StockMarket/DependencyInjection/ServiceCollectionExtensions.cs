using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.StockMarket.Implementation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Effanville.TradingStructures.StockMarket.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimulationExchange(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddOptions<StockMarketAdapterSettings>();
        return serviceCollection
            .AddSingleton(sp => sp.GetRequiredService<IOptions<StockMarketAdapterSettings>>().Value)
            .AddSingleton<IStockMarketAdapter, SimulationExchange>()
            .AddSingleton<IService>(x => x.GetService<IStockMarketAdapter>()!);
    }
}
