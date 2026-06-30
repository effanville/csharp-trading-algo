using Effanville.TradingStructures.Strategies.Decision;

namespace Effanville.TradingStructures.Strategies;

public sealed record StrategySettings(SingleStrategySettings[] Settings);

public sealed record SingleStrategySettings(
    string Name,
    DecisionSystemFactory.Settings DecisionParameters);
