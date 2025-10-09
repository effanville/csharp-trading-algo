
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.DependencyInjection;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingSystem.MarketEvolvers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingSystem.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static ILoggingBuilder RegisterLogging(
        this ILoggingBuilder loggingBuilder,
        IReportLogger? reportLogger = null)
    {
        if (reportLogger == null)
        {
            _ = loggingBuilder
                .ClearProviders()
                .AddReportLogger(config => config.MinimumLogLevel = ReportType.Information);
        }
        else
        {
            _ = loggingBuilder.AddReportLogger(reportLogger);
        }
        return loggingBuilder;
    }

    public static IServiceCollection RegisterTradingServices(
        this IServiceCollection serviceCollection,
        string stockFilePath,
        DateTime startTime,
        DateTime endTime,
        TimeSpan evolutionIncrement,
        StrategySettings strategySettings,
        IFileSystem? fileSystem = null)
    {
        if (fileSystem == null)
        {
            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
        }
        else
        {
            _ = serviceCollection.AddSingleton(fileSystem);
        }

        _ = serviceCollection.AddSingleton(
            x => CreateExchange(
                stockFilePath,
                x.GetService<IFileSystem>()!,
                x.GetService<IReportLogger>()!));

        _ = serviceCollection.AddSingleton(
            x => new TimeIncrementEvolverSettings(
                startTime,
                endTime,
                evolutionIncrement));
        _ = serviceCollection.AddSingleton<EvolverSettings>(
            x => x.GetService<TimeIncrementEvolverSettings>()!);

        _ = serviceCollection.AddStrategy(strategySettings);
        _ = serviceCollection.AddSingleton<IEventEvolver, EventEvolver>();
        return serviceCollection.AddHostedService<TradingSystemHostedService>();
    }

    public static async Task<StrategyHistory> RunSystemAsync(this IHost host)
    {
        var reportLogger = host.Services.GetService<IReportLogger>()!;
        var evolver = host.Services.GetService<IEventEvolver>()!;
        using (new Timer(reportLogger, "Execution"))
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

    private static IStockExchange CreateExchange(string filePath, IFileSystem fileSystem, IReportLogger logger)
    {
        using (new Timer(logger, "Loading Exchange"))
        {
            var exchange = StockExchangeFactory.Create(filePath, fileSystem, logger);
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
