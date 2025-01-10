using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;
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
        string stockFilePath,
        DateTime startTime,
        DateTime endTime,
        TimeSpan evolutionIncrement,
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings,
        DecisionSystemFactory.Settings decisionParameters,
        IFileSystem? fileSystem = null)
    {
        if (fileSystem == null)
        {
            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
        }
        else
        {
            serviceCollection.AddSingleton(fileSystem);
        }

        serviceCollection.AddSingleton<IStockExchange>(
            x => CreateExchange(
                stockFilePath,
                x.GetService<IFileSystem>()!,
                x.GetService<IReportLogger>()!));
        serviceCollection.AddSingleton<TimeIncrementEvolverSettings>(
            x => new TimeIncrementEvolverSettings(
                startTime,
                endTime,
                evolutionIncrement,
                x.GetService<IStockExchange>()!));
        serviceCollection.AddSingleton<EvolverSettings>(
            x => x.GetService<TimeIncrementEvolverSettings>()!);

        serviceCollection.AddSingleton<IDecisionSystem>(
            x => CreateDecisionSystem(
                x.GetService<TimeIncrementEvolverSettings>()!,
                decisionParameters,
                x.GetService<IReportLogger>()!));

        serviceCollection.AddSingleton<IPortfolioManager>(
            x => CreatePortfolioManager(
                x.GetService<IFileSystem>()!,
                startSettings,
                constructionSettings,
                x.GetService<IReportLogger>()!));
        serviceCollection.AddSingleton<IExecutionStrategy>(
            x => ExecutionStrategyFactory.Create(
                StrategyType.ExchangeOpen,
                x.GetService<IReportLogger>()!,
                x.GetService<IStockExchange>()!,
                x.GetService<IDecisionSystem>()!));
        serviceCollection.AddSingleton<IStrategy, Strategy>();
        serviceCollection.AddSingleton<IEventEvolver, EventEvolver>();
        serviceCollection.AddHostedService<TradingSystemHostedService>();
        return serviceCollection;
    }

    public static async Task<EvolverResult> RunSystemAsync(this IHost host)
    {
        var reportLogger = host.Services.GetService<IReportLogger>();
        var evolver = host.Services.GetService<IEventEvolver>();
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

    private static IDecisionSystem CreateDecisionSystem(
        TimeIncrementEvolverSettings simulatorSettings,
        DecisionSystemFactory.Settings decisionParameters,
        IReportLogger reportLogger)
    {
        using (new Timer(reportLogger, "Calibrating"))
        {
            DecisionSystemSettings decisionSettings = new DecisionSystemSettings(
                simulatorSettings.BurnInStart,
                simulatorSettings.StartTime,
                simulatorSettings.Exchange.Stocks.Count,
                simulatorSettings.Exchange);
            IDecisionSystem decisionSystem = DecisionSystemFactory.CreateAndCalibrate(
                decisionParameters,
                decisionSettings,
                reportLogger);
            if (decisionSettings.BurnInEnd == decisionSettings.StartTime)
            {
                simulatorSettings.DoesntRequireBurnIn();
            }

            return decisionSystem;
        }
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

    private static IPortfolioManager CreatePortfolioManager(IFileSystem fileSystem,
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings,
        IReportLogger logger)
    {
        using (new Timer(logger, "Loading Portfolio"))
        {
            return PortfolioManager.LoadFromFile(fileSystem, startSettings, constructionSettings, logger);
        }
    }
}