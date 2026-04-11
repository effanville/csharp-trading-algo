using System.IO.Abstractions;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Strategies.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrategy(this IServiceCollection serviceCollection,
        DecisionSystemFactory.Settings decisionParameters,
        PortfolioStartSettings startSettings,
        PortfolioConstructionSettings constructionSettings)
    {
        return serviceCollection
            .AddSingleton(
                x => CreateDecisionSystem(
                    decisionParameters,
                    x.GetRequiredService<IReportLogger>()))
            .AddSingleton(
                x => CreatePortfolioManager(
                    x.GetRequiredService<IFileSystem>(),
                    startSettings,
                    constructionSettings,
                    x.GetRequiredService<IReportLogger>()));
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