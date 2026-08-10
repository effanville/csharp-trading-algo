using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Effanville.TradingStructures.Strategies.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrategy(
        this IServiceCollection serviceCollection,
        IConfigurationManager config)
    {
        _ = serviceCollection
            .AddOptions<StrategySettings>();
        _ = serviceCollection
            .AddSingleton<IDecisionSystemFactory, DecisionSystemFactory>()
            .AddSingleton<IPortfolioManagerFactory, PortfolioManagerFactory>()
            .AddSingleton<IExecutionStrategyFactory, ExecutionStrategyFactory>()
            .AddSingleton<IStockSelector, StockSelector>();
        StrategySettings? options = config
            .GetSection(nameof(StrategySettings))
            .Get<StrategySettings>();
        foreach (string strategyName in options.StrategyNames)
        {
            _ = serviceCollection.AddOptions<PortfolioStartSettings>(strategyName)
                .BindConfiguration($"{strategyName}:{PortfolioStartSettings.OptionsName}");
            _ = serviceCollection.AddOptions<PortfolioConstructionSettings>(strategyName)
                .BindConfiguration($"{strategyName}:{PortfolioConstructionSettings.OptionsName}");
            _ = serviceCollection.AddOptions<DecisionSystemFactory.Settings>(strategyName)
                .BindConfiguration($"{strategyName}:{DecisionSystemFactory.Settings.OptionsName}");
            _ = serviceCollection.AddOptions<StockSelectorSettings>(strategyName)
                .BindConfiguration($"{strategyName}:{StockSelectorSettings.OptionsName}");

            _ = serviceCollection.AddSingleton<IStrategy, Strategy>(x => ResolveStrategy(x, strategyName));
        }

        return serviceCollection;
    }

    private static Strategy ResolveStrategy(this IServiceProvider sp, string strategyName)
    {
        IDecisionSystemFactory decisionSystemFactory = sp.GetRequiredService<IDecisionSystemFactory>();
        IOptionsSnapshot<DecisionSystemFactory.Settings> decisionOptions = sp.GetRequiredService<IOptionsSnapshot<DecisionSystemFactory.Settings>>();
        DecisionSystemFactory.Settings decisionSettings = decisionOptions.Get(strategyName);
        IDecisionSystem decisionSystem = decisionSystemFactory.Create(decisionSettings);

        IPortfolioManagerFactory portfolioManagerFactory = sp.GetRequiredService<IPortfolioManagerFactory>();

        IOptionsSnapshot<PortfolioStartSettings> portfolioStartOptions = sp.GetRequiredService<IOptionsSnapshot<PortfolioStartSettings>>();
        PortfolioStartSettings portfolioStartSettings = portfolioStartOptions.Get(strategyName);

        IOptionsSnapshot<PortfolioConstructionSettings> portfolioConstructionOptions = sp.GetRequiredService<IOptionsSnapshot<PortfolioConstructionSettings>>();
        PortfolioConstructionSettings portfolioConstructionSettings = portfolioConstructionOptions.Get(strategyName);

        IOptionsSnapshot<StockSelectorSettings> stockSelectorOptions = sp.GetRequiredService<IOptionsSnapshot<StockSelectorSettings>>();
        StockSelectorSettings stockSelectorSettings = stockSelectorOptions.Get(strategyName);

        IPortfolioManager portfolioManager = portfolioManagerFactory.LoadFromFile(portfolioStartSettings, portfolioConstructionSettings);

        IStockSelector stockSelector = sp.GetRequiredService<IStockSelector>();

        IExecutionStrategyFactory factory = sp.GetRequiredService<IExecutionStrategyFactory>();
        IExecutionStrategy executionStrategy = factory.Create(
            StrategyType.ExchangeOpen,
            sp.GetRequiredService<IStockExchange>(),
            decisionSystem);

        ILogger<Strategy> logger = sp.GetRequiredService<ILogger<Strategy>>();
        return new Strategy(executionStrategy, portfolioManager, stockSelector, logger);
    }
}
