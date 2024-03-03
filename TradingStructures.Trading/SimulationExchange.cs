using System;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Trading.Implementation;

namespace Effanville.TradingStructures.Trading
{
    public class SimulationExchange : IMarketExchange, IService
    {
        private readonly IClock _clock;
        private readonly IPriceService _priceService;
        private readonly ITradeSubmitter _tradeSubmitter;
        private readonly IReportLogger _logger;

        public string Name => nameof(SimulationExchange);
        public TradeMechanismSettings Settings { get; }

        public SimulationExchange(
            TradeMechanismSettings settings, 
            IPriceService priceService, 
            IClock clock, 
            IReportLogger logger)
        {
            Settings = settings;
            _priceService = priceService;
            _clock = clock;
            _tradeSubmitter = new SimulationBuySellSystem(settings);
            _logger = logger;
        }

        public void Initialize(EvolverSettings settings) { }

        public void Restart() { }

        public void Shutdown() { }
        public void OnTradeRequested(object obj, TradeSubmittedEventArgs eventArgs)
        {
            DateTime time = _clock.UtcNow();
            Trade trade = eventArgs.RequestedTrade;
            var validatedTrade = _tradeSubmitter.Trade(time, trade, _priceService, eventArgs.AvailableFunds, _logger);
            if (validatedTrade != null)
            {
                TradeCompleted?.Invoke(null, new TradeCompletedEventArgs(trade, validatedTrade, true));
            }
            else
            {
                TradeCompleted?.Invoke(null, new TradeCompletedEventArgs(trade, null, false));
            }
        }

        public event EventHandler<TradeCompletedEventArgs> TradeCompleted;
    }
}