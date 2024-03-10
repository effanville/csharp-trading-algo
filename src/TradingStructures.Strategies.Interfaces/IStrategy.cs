using System;

using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingStructures.Strategies;

public interface IStrategy : IService
{
    /// <summary>
    /// Event to subscribe to for the dealing with Trades created.
    /// </summary>
    event EventHandler<TradeSubmittedEventArgs> SubmitTradeEvent;
    public IDecisionSystem DecisionSystem { get; }
    public IExecutionStrategy ExecutionStrategy {get;}
    public IPortfolioManager PortfolioManager { get; }
    
    /// <summary>
    /// Event that fires every short time period for checking strategy.
    /// </summary>
    void OnTimeIncrementUpdate(object? obj, TimeIncrementEventArgs eventArgs);
    
    /// <summary>
    /// Event that is called at the point of the status of an exchange changing.
    /// </summary>
    void OnExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs);
    
    /// <summary>
    /// Event that is called at the point of a price change occurring.
    /// </summary>
    void OnPriceUpdate(object? obj, PriceUpdateEventArgs eventArgs);
}