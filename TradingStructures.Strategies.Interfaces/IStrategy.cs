using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;

namespace Effanville.TradingStructures.Strategies;

public interface IStrategy : IService
{
    public IDecisionSystem DecisionSystem { get; }
    public IExecutionStrategy ExecutionStrategy {get;}
    public IPortfolioManager PortfolioManager { get; }
    
    /// <summary>
    /// Event that fires every short time period for checking strategy.
    /// </summary>
    void OnTimeIncrementUpdate(object obj, TimeIncrementEventArgs eventArgs);
}