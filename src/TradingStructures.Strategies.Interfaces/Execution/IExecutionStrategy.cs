using System;

using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingStructures.Strategies.Execution;

/// <summary>
/// Contains all necessary methods for the execution of a stock market trading strategy
/// </summary>
public interface IExecutionStrategy : IService
{
    /// <summary>
    /// Event to subscribe to for the dealing with Trades created.
    /// </summary>
    event EventHandler<TradeSubmittedEventArgs> SubmitTradeEvent;

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