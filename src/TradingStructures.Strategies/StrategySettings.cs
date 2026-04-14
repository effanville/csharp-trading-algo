using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;

namespace Effanville.TradingStructures.Strategies;

public sealed record StrategySettings(
    PortfolioStartSettings StartSettings,
    PortfolioConstructionSettings ConstructionSettings,
    DecisionSystemFactory.Settings DecisionParameters);
