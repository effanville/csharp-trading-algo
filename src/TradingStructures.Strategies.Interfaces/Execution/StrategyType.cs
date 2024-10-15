namespace Effanville.TradingStructures.Strategies.Execution;

public enum StrategyType : byte
{
    /// <summary>
    /// Default type which only logs information on the price and exchange events.
    /// </summary>
    LogExecution,

    /// <summary>
    /// All calculation behaviour is carried out upon exchange open.
    /// </summary>
    ExchangeOpen,
    
    /// <summary>
    /// All calculation behaviour is carried out in an exchange event.
    /// </summary>
    ExchangeEvent
}