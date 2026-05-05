using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrategy(
        this IServiceCollection serviceCollection,
        StrategySettings strategySettings)
    {
        _ = serviceCollection
            .AddSingleton<IDecisionSystemFactory, DecisionSystemFactory>()
            .AddSingleton<IPortfolioManagerFactory, PortfolioManagerFactory>()
            .AddSingleton<IExecutionStrategyFactory, ExecutionStrategyFactory>();
        foreach (SingleStrategySettings settings in strategySettings.Settings)
        {
            _ = serviceCollection
                .AddSingleton<IStrategy, Strategy>(
                    x =>
                    {
                        IDecisionSystemFactory decisionSystemFactory = x.GetRequiredService<IDecisionSystemFactory>();
                        IDecisionSystem decisionSystem = decisionSystemFactory.Create(settings.DecisionParameters);

                        IPortfolioManagerFactory portfolioManagerFactory = x.GetRequiredService<IPortfolioManagerFactory>();
                        IPortfolioManager portfolioManager = portfolioManagerFactory.LoadFromFile(settings.StartSettings, settings.ConstructionSettings);

                        IExecutionStrategyFactory factory = x.GetRequiredService<IExecutionStrategyFactory>();
                        IExecutionStrategy executionStrategy = factory.Create(
                            StrategyType.ExchangeOpen,
                            x.GetRequiredService<IStockExchange>(),
                            decisionSystem);

                        ILogger<Strategy> logger = x.GetRequiredService<ILogger<Strategy>>();
                        return new Strategy(executionStrategy, portfolioManager, logger);
                    });
        }

        return serviceCollection;
    }
}
