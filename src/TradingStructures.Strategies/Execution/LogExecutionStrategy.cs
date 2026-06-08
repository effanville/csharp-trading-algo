using System;

using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.MarketData;
using Effanville.TradingStructures.OrderManagement;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Execution
{
    public class LogExecutionStrategy : IExecutionStrategy
    {
        public event EventHandler<TradeSubmittedEventArgs>? SubmitTradeEvent;
        private readonly ILogger<LogExecutionStrategy> _logger;

        private readonly IClock _clock;

        public LogExecutionStrategy(ILogger<LogExecutionStrategy> logger, IClock clock)
        {
            _logger = logger;
            _clock = clock;
        }

        /// <inheritdoc/>
        public void OnTimeIncrementUpdate(object? obj, TimeIncrementEventArgs eventArgs)
            => _logger.LogInformation($"TimeIncrement occurred. Time now is {eventArgs.Time.ToUniversalTime()}");

        /// <inheritdoc/>
        public void OnPriceUpdate(object? obj, PriceUpdateEventArgs eventArgs)
            => _logger.LogInformation($"Price for {eventArgs.Instrument.Ticker} has changed to {eventArgs.Price} at " +
                $"time {eventArgs.Time}, actual time  {_clock?.UtcNow()}");

        /// <inheritdoc/>
        public void OnExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs)
            => _logger.LogInformation($"Exchange session changed from {eventArgs.PreviousSession} to {eventArgs.NewSession} at " +
                $"time {eventArgs.Time}, actual time {_clock?.UtcNow()}");
    }
}