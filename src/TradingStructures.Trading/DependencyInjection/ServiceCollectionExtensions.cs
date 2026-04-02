using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Trading.Implementation;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Trading.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimulationExchange(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(a => TradeMechanismSettings.Default())
            .AddSingleton<IMarketExchange, SimulationExchange>()
            .AddSingleton<IService>(x => x.GetService<IMarketExchange>()!);
    }
}
