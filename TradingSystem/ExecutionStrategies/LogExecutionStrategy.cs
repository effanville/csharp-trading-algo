using System;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;

using TradingSystem.ExchangeStructures;
using TradingSystem.MarketEvolvers;
using TradingSystem.PriceSystem;
using TradingSystem.Time;
using TradingSystem.Trading;

namespace TradingSystem.ExecutionStrategies
{
    public class LogExecutionStrategy : IExecutionStrategy
    {
        public event EventHandler<TradeSubmittedEventArgs> SubmitTradeEvent;
        private readonly IReportLogger _logger;
        private readonly IClock _clock;

        public string Name => throw new NotImplementedException();

        public LogExecutionStrategy(IClock clock, IReportLogger logger)
        {
            _clock = clock;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Initialize(EvolverSettings settings)
        {
        }

        public void Restart() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void OnTimeIncrementUpdate(object obj, TimeIncrementEventArgs eventArgs)
            => _logger.Log(ReportType.Information, "TimeUpdate", $"TimeIncrement occurred. Time now is {eventArgs.Time.ToUniversalTime()}");

        /// <inheritdoc/>
        public void OnPriceUpdate(object obj, PriceUpdateEventArgs eventArgs)
            => _logger.Log(ReportType.Information, "PriceService", $"Price for {eventArgs.Instrument.Ticker} has changed to {eventArgs.Price} at time {_clock.UtcNow()}");

        /// <inheritdoc/>
        public void OnExchangeStatusChanged(object obj, ExchangeStatusChangedEventArgs eventArgs)
            => _logger.Log(ReportType.Information, "ExchangeService", $"Exchange session changed from {eventArgs.PreviousSession} to {eventArgs.NewSession} at time {_clock.UtcNow()}");

        /// <inheritdoc/>
        public void Shutdown()
        {
        }
    }
}