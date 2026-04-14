using System.IO.Abstractions;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Strategies.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrategy(
        this IServiceCollection serviceCollection,
        StrategySettings strategySettings)
    {
        return serviceCollection
            .AddSingleton(
                x => CreateDecisionSystem(
                    strategySettings.DecisionParameters,
                    x.GetRequiredService<IReportLogger>()))
            .AddSingleton(
                x => CreatePortfolioManager(
                    x.GetRequiredService<IFileSystem>(),
                    strategySettings.StartSettings,
                    strategySettings.ConstructionSettings,
                    x.GetRequiredService<IReportLogger>()))
            .AddSingleton(
            x => ExecutionStrategyFactory.Create(
                StrategyType.ExchangeOpen,
                x.GetRequiredService<IReportLogger>(),
                x.GetRequiredService<IStockExchange>(),
                x.GetRequiredService<IDecisionSystem>()))
            .AddSingleton<IStrategy, Strategy>();
    }

    private static IDecisionSystem CreateDecisionSystem(
        DecisionSystemFactory.Settings decisionParameters,
        IReportLogger reportLogger)
    {
        using (new Timer(reportLogger, "Calibrating"))
        {
            IDecisionSystem decisionSystem = DecisionSystemFactory.Create(
                decisionParameters);

            return decisionSystem;
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
