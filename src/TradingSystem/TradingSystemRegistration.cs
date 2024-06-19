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

namespace Effanville.TradingSystem;

public static class TradingSystemRegistration
{
    public static EvolverResult SetupAndRun(
        string stockFilePath,
        DateTime startTime,
        DateTime endTime,
        TimeSpan evolutionIncrement,
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings,
        DecisionSystemFactory.Settings decisionParameters,
        IFileSystem? fileSystem = null,
        IReportLogger? reportLogger = null)
    {
        var builder = new HostApplicationBuilder();
        builder.SetupSystem(
            stockFilePath,
            startTime,
            endTime,
            evolutionIncrement,
            startSettings,
            constructionSettings,
            decisionParameters,
            fileSystem,
            reportLogger);
        var host = builder.Build();
        return RunSystem(
            host.Services.GetService<IEventEvolver>(),
            host.Services.GetService<IReportLogger>());
    }

    public static EvolverResult RunSystem(IEventEvolver evolver, IReportLogger reportLogger)
    {
        using (new Timer(reportLogger, "Execution"))
        {
            evolver.Initialise();
            evolver.Start();
            while (evolver.IsActive)
            {
                _ = Task.Delay(100);
            }
        }

        return evolver.Result;
    }

    public static HostApplicationBuilder SetupSystem(
        this HostApplicationBuilder hostApplicationBuilder,
        string stockFilePath,
        DateTime startTime,
        DateTime endTime,
        TimeSpan evolutionIncrement,
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings,
        DecisionSystemFactory.Settings decisionParameters,
        IFileSystem? fileSystem = null,
        IReportLogger? reportLogger = null)
    {
        if (reportLogger == null)
        {
            hostApplicationBuilder.Logging
                .ClearProviders()
                .AddReportLogger(config => config.MinimumLogLevel = ReportType.Information);
        }
        else
        {
            hostApplicationBuilder.Logging.AddReportLogger(reportLogger);
        }

        if (fileSystem == null)
        {
            hostApplicationBuilder.Services.AddSingleton<IFileSystem, FileSystem>();
        }
        else
        {
            hostApplicationBuilder.Services.AddSingleton(fileSystem);
        }

        hostApplicationBuilder.Services.AddTradingSystem(
            stockFilePath,
            startTime,
            endTime,
            evolutionIncrement,
            startSettings,
            constructionSettings,
            decisionParameters);
        return hostApplicationBuilder;
    }

    private static IServiceCollection AddTradingSystem(
        this IServiceCollection serviceCollection,
        string stockFilePath,
        DateTime startTime,
        DateTime endTime,
        TimeSpan evolutionIncrement,
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings,
        DecisionSystemFactory.Settings decisionParameters)
    {
        serviceCollection.AddSingleton<IStockExchange>(
            x => CreateExchange(
                stockFilePath,
                x.GetService<IFileSystem>(),
                x.GetService<IReportLogger>()));
        serviceCollection.AddSingleton<TimeIncrementEvolverSettings>(
            x => new TimeIncrementEvolverSettings(
                startTime,
                endTime,
                evolutionIncrement,
                x.GetService<IStockExchange>()));
        serviceCollection.AddSingleton<EvolverSettings>(
            x => x.GetService<TimeIncrementEvolverSettings>());

        serviceCollection.AddSingleton<IDecisionSystem>(
            x => CreateDecisionSystem(
                x.GetService<TimeIncrementEvolverSettings>(),
                decisionParameters,
                x.GetService<IReportLogger>()));

        serviceCollection.AddSingleton<IPortfolioManager>(
            x => CreatePortfolioManager(
                x.GetService<IFileSystem>(),
                startSettings,
                constructionSettings,
                x.GetService<IReportLogger>()));
        serviceCollection.AddSingleton<IExecutionStrategy>(
            x => ExecutionStrategyFactory.Create(
                StrategyType.ExchangeOpen,
                x.GetService<IReportLogger>(),
                x.GetService<IStockExchange>(),
                x.GetService<IDecisionSystem>()));
        serviceCollection.AddSingleton<IStrategy, Strategy>();
        serviceCollection.AddSingleton<IEventEvolver, EventEvolver>();
        return serviceCollection;
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
            return StockExchangeFactory.Create(filePath, fileSystem, logger);
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