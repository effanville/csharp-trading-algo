using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;

namespace Effanville.TradingStructures.Strategies;

public sealed class StrategySettings
{
    public PortfolioStartSettings StartSettings { get; }

    public PortfolioConstructionSettings ConstructionSettings { get; }
    public DecisionSystemFactory.Settings DecisionParameters { get; }

    public StrategySettings(PortfolioStartSettings startSettings, PortfolioConstructionSettings constructionSettings, DecisionSystemFactory.Settings decisionParameters)
    {
        StartSettings = startSettings;
        ConstructionSettings = constructionSettings;
        DecisionParameters = decisionParameters;
    }
}
