using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Pricing.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPriceService(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(a => PriceCalculationSettings.Default())
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
