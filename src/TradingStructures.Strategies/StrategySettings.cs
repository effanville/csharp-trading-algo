namespace Effanville.TradingStructures.Strategies;

public sealed record StrategySettings(SingleStrategySettings[] Settings);

public sealed record SingleStrategySettings(
    string Name);
