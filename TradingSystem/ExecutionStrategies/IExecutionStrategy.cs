﻿using System;

using TradingSystem.ExchangeStructures;
using TradingSystem.PriceSystem;
using TradingSystem.Time;
using TradingSystem.Trading;

namespace TradingSystem.ExecutionStrategies;

/// <summary>
/// Contains all necessary methods for the execution of a stock market trading strategy
/// </summary>
public interface IExecutionStrategy
{
    /// <summary>
    /// Event to subscribe to for the dealing with Trades created.
    /// </summary>
    event EventHandler<TradeSubmittedEventArgs> SubmitTradeEvent;

    /// <summary>
    /// Perform any setup required of the strategy.
    /// </summary>
    void Initialise();

    /// <summary>
    /// Event that fires every short time period for checking strategy.
    /// </summary>
    void OnTimeIncrementUpdate(object obj, TimeIncrementEventArgs eventArgs);

    /// <summary>
    /// Event that is called at the point of the status of an exchange changing.
    /// </summary>
    void OnExchangeStatusChanged(object obj, ExchangeStatusChangedEventArgs eventArgs);

    /// <summary>
    /// Event that is called at the point of a price change occurring.
    /// </summary>
    void OnPriceUpdate(object obj, PriceUpdateEventArgs eventArgs);

    /// <summary>
    /// Perform any cleanup at the end of the execution.
    /// </summary>
    void Shutdown();
}