using System;

using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.MarketData;
using Effanville.TradingStructures.OrderManagement;

namespace Effanville.TradingStructures.Strategies;

public interface IStrategy : IService
{
    /// <summary>
    /// Event to subscribe to for the dealing with Trades created.
    /// </summary>
    event EventHandler<TradeSubmittedEventArgs> SubmitTradeEvent;

    public StrategyHistory History { get; }

    /// <summary>
    /// Register the simulation services for the strategy.
    /// </summary>
    bool RegisterServices(IServiceProvider serviceProvider);

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

    /// <summary>
    /// Adds a trade into the portfolio.
    /// </summary>
    void OnTradeConfirmed(object? obj, TradeCompletedEventArgs eventArgs);
}