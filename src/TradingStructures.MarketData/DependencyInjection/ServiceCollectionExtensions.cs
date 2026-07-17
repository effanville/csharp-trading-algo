using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Effanville.TradingStructures.MarketData.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPriceService(
        this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddOptions<PriceCalculationSettings>();
        return serviceCollection
            .AddSingleton(sp => sp.GetRequiredService<IOptions<PriceCalculationSettings>>().Value)
            .AddSingleton<IPriceServiceFactory, PriceServiceFactory>()
            .AddSingleton(a =>
            {
                var factory = a.GetService<IPriceServiceFactory>()!;
                var settings = a.GetService<PriceCalculationSettings>()!;
                var exchange = a.GetService<IStockExchange>()!;
                var scheduler = a.GetService<IScheduler>()!;
                return factory.Create(settings, exchange, scheduler);
            })
            .AddSingleton<IService>(a => a.GetService<IPriceService>()!);
    }
}
