using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies;

using Effanville.TradingStructures.Strategies.DependencyInjection;


using Effanville.TradingSystem.MarketEvolvers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingSystem.DependencyInjection;

public static class RegistrationExtensions
{
    public static ILoggingBuilder RegisterLogging(
        this ILoggingBuilder loggingBuilder,
        IReportLogger? reportLogger = null)
    {
        if (reportLogger == null)
        {
            loggingBuilder
                .ClearProviders()
                .AddReportLogger(config => config.MinimumLogLevel = ReportType.Information);
        }
        else
        {
            loggingBuilder.AddReportLogger(reportLogger);
        }
        return loggingBuilder;
    }

    public static IServiceCollection RegisterTradingServices(
        this IServiceCollection serviceCollection,
        EvolverSettings settings,
        StrategySettings strategySettings,
        IFileSystem? fileSystem)
    {
        serviceCollection.AddSingleton(fileSystem);
        serviceCollection.AddSingleton<ITimerFactory, TimerFactory>();

        serviceCollection
            .AddSingleton<IStockExchangeFactory, StockExchangeFactory>()
            .AddSingleton(
            x =>
            {
                var timerFactory = x.GetRequiredService<ITimerFactory>();
                return CreateExchange(
                                settings.StockFilePath,
                                x.GetRequiredService<IStockExchangeFactory>(),
                                timerFactory);
            });
        return serviceCollection
            .AddSingleton(settings)
            .AddStrategy(
            strategySettings)
            .AddSingleton<IEventEvolver, EventEvolver>()
            .AddHostedService<TradingSystemHostedService>();
    }

    public static async Task<StrategyHistory> RunSystemAsync(this IHost host)
    {
        var timerFactory = host.Services.GetRequiredService<ITimerFactory>();
        var evolver = host.Services.GetRequiredService<IEventEvolver>();
        using (timerFactory.Create("Execution"))
        {
            evolver.Initialise();
            evolver.Start();
            while (evolver.IsActive)
            {
                await Task.Delay(100);
            }
        }

        return evolver.Result;
    }

    private static IStockExchange CreateExchange(string filePath, IStockExchangeFactory stockExchangeFactory, ITimerFactory timerFactory)
    {
        using (timerFactory.Create("Loading Exchange"))
        {
            var exchange = stockExchangeFactory.Create(new (filePath));
            foreach (var stock in exchange.Stocks)
            {
                foreach (var value in stock.Valuations)
                {
                    value.Start = DateTime.SpecifyKind(value.Start, DateTimeKind.Utc);
                }
            }

            return exchange;
        }
    }
}
