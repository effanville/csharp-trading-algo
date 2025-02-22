﻿using System;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingStructures.Strategies.Execution
{
    public class LogExecutionStrategy : IExecutionStrategy
    {
        public event EventHandler<TradeSubmittedEventArgs>? SubmitTradeEvent;
        private readonly IReportLogger _logger;
        private readonly IClock _clock;

        public string Name => nameof(LogExecutionStrategy);

        public LogExecutionStrategy(IClock clock, IReportLogger logger)
        {
            _clock = clock;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Initialize(EvolverSettings settings)
        {
        }

        public void Restart() { }

        /// <inheritdoc/>
        public void OnTimeIncrementUpdate(object? obj, TimeIncrementEventArgs eventArgs)
            => _logger.Log(ReportType.Information, "TimeUpdate", $"TimeIncrement occurred. Time now is {eventArgs.Time.ToUniversalTime()}");

        /// <inheritdoc/>
        public void OnPriceUpdate(object? obj, PriceUpdateEventArgs eventArgs)
            => _logger.Log(ReportType.Information, "PriceService", $"Price for {eventArgs.Instrument.Ticker} has changed to {eventArgs.Price} at time {_clock.UtcNow()}");

        /// <inheritdoc/>
        public void OnExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs)
            => _logger.Log(ReportType.Information, "ExchangeService", $"Exchange session changed from {eventArgs.PreviousSession} to {eventArgs.NewSession} at time {_clock.UtcNow()}");

        /// <inheritdoc/>
        public void Shutdown()
        {
        }
    }
}