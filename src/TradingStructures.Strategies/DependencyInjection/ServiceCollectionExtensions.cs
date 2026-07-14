using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            _ = serviceCollection.AddOptions<PortfolioStartSettings>(settings.Name)
                .BindConfiguration($"{settings.Name}:{PortfolioStartSettings.OptionsName}");
            _ = serviceCollection.AddOptions<PortfolioConstructionSettings>(settings.Name)
                .BindConfiguration($"{settings.Name}:{PortfolioConstructionSettings.OptionsName}");
            _ = serviceCollection.AddOptions<DecisionSystemFactory.Settings>(settings.Name)
                .BindConfiguration($"{settings.Name}:{DecisionSystemFactory.Settings.OptionsName}");

            _ = serviceCollection
                .AddSingleton<IStrategy, Strategy>(
                    x => ResolveStrategy(x, settings.Name));
        }

        return serviceCollection;
    }

    private static Strategy ResolveStrategy(this IServiceProvider sp, string name)
    {
        IDecisionSystemFactory decisionSystemFactory = sp.GetRequiredService<IDecisionSystemFactory>();
        IOptionsSnapshot<DecisionSystemFactory.Settings> decisionOptions = sp.GetRequiredService<IOptionsSnapshot<DecisionSystemFactory.Settings>>();
        DecisionSystemFactory.Settings decisionSettings = decisionOptions.Get(name);
        IDecisionSystem decisionSystem = decisionSystemFactory.Create(decisionSettings);

        IPortfolioManagerFactory portfolioManagerFactory = sp.GetRequiredService<IPortfolioManagerFactory>();

        IOptionsSnapshot<PortfolioStartSettings> portfolioStartOptions = sp.GetRequiredService<IOptionsSnapshot<PortfolioStartSettings>>();
        PortfolioStartSettings portfolioStartSettings = portfolioStartOptions.Get(name);

        IOptionsSnapshot<PortfolioConstructionSettings> portfolioConstructionOptions = sp.GetRequiredService<IOptionsSnapshot<PortfolioConstructionSettings>>();
        PortfolioConstructionSettings portfolioConstructionSettings = portfolioConstructionOptions.Get(name);


        IPortfolioManager portfolioManager = portfolioManagerFactory.LoadFromFile(portfolioStartSettings, portfolioConstructionSettings);

        IExecutionStrategyFactory factory = sp.GetRequiredService<IExecutionStrategyFactory>();
        IExecutionStrategy executionStrategy = factory.Create(
            StrategyType.ExchangeOpen,
            sp.GetRequiredService<IStockExchange>(),
            decisionSystem);

        ILogger<Strategy> logger = sp.GetRequiredService<ILogger<Strategy>>();
        return new Strategy(executionStrategy, portfolioManager, logger);
    }
}
