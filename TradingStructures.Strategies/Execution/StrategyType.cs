namespace Effanville.TradingStructures.Strategies.Execution;

public enum StrategyType : byte
{
    /// <summary>
    /// Default type which only logs information on the price and exchange events.
    /// </summary>
    LogExecution,

    /// <summary>
    /// Mimicks the manner in which a Time increment evolver over a day would work.
    /// </summary>
    TimeIncrementExecution
}