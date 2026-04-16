using Effanville.FinancialStructures.Stocks;
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
            .AddSingleton<IDecisionSystemFactory, DecisionSystemFactory>()
            .AddSingleton(
                x =>
                {
                    IDecisionSystemFactory decisionSystemFactory = x.GetRequiredService<IDecisionSystemFactory>();
                    return decisionSystemFactory.Create(strategySettings.DecisionParameters);
                })
            .AddSingleton<IPortfolioManagerFactory, PortfolioManagerFactory>()
            .AddSingleton(
                x =>
                {
                    IPortfolioManagerFactory portfolioManagerFactory = x.GetRequiredService<IPortfolioManagerFactory>();
                    return portfolioManagerFactory.LoadFromFile(strategySettings.StartSettings, strategySettings.ConstructionSettings);
                })
            .AddSingleton<IExecutionStrategyFactory, ExecutionStrategyFactory>()
            .AddSingleton(
                x =>
                {
                    IExecutionStrategyFactory factory = x.GetRequiredService<IExecutionStrategyFactory>();
                    return factory.Create(
                        StrategyType.ExchangeOpen,
                        x.GetRequiredService<IStockExchange>(),
                        x.GetRequiredService<IDecisionSystem>());
                })
            .AddSingleton<IStrategy, Strategy>();
    }
}
