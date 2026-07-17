namespace Effanville.TradingStructures.Strategies;

public sealed record StrategySettings
{
    public const string OptionsName = nameof(StrategySettings);
    public string[]? StrategyNames { get; set; }
}
