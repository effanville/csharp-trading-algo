using System.IO.Abstractions;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Strategies.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrategy(this IServiceCollection serviceCollection,
        StrategySettings strategySettings)
    {
        _ = serviceCollection.AddSingleton<IDecisionSystem>(
            x => CreateDecisionSystem(
                strategySettings.DecisionParameters,
                x.GetService<IReportLogger>()!));
        _ = serviceCollection.AddPortfolioManager(strategySettings.StartSettings, strategySettings.ConstructionSettings);
        _ = serviceCollection.AddSingleton<IExecutionStrategy>(
            x => ExecutionStrategyFactory.Create(
                StrategyType.ExchangeOpen,
                x.GetService<IReportLogger>()!,
                x.GetService<IDecisionSystem>()!));
        _ = serviceCollection.AddSingleton<IStrategy, Strategy>();
        return serviceCollection;
    }

    public static IServiceCollection AddPortfolioManager(
        this IServiceCollection serviceCollection,
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings)
        => serviceCollection.AddSingleton<IPortfolioManager>(
        x => CreatePortfolioManager(
            x.GetService<IFileSystem>()!,
            startSettings,
            constructionSettings,
            x.GetService<IReportLogger>()!));

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

    private static IDecisionSystem CreateDecisionSystem(
        DecisionSystemFactory.Settings decisionParameters,
        IReportLogger reportLogger)
    {
        using (new Timer(reportLogger, "Calibrating"))
        {
            return DecisionSystemFactory.Create(decisionParameters);
        }
    }
}
