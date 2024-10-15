namespace Effanville.TradingStructures.Strategies.Execution;

public sealed class ExecutionSettings
{
    public StrategyType StrategyType { get; }
    
    public ExecutionSettings(StrategyType strategyType)
    {
        StrategyType = strategyType;
    }
}