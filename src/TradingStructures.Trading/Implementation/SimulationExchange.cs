using System;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;

namespace Effanville.TradingStructures.Trading.Implementation
{
    public class SimulationExchange : IMarketExchange, IService
    {
        private readonly IClock? _clock;
        private readonly IPriceService? _priceService;
        private readonly IReportLogger _logger;
        private readonly TradeMechanismSettings _settings;

        public event EventHandler<TradeCompletedEventArgs>? TradeCompleted;
        
        public string Name => nameof(SimulationExchange);

        public SimulationExchange(
            TradeMechanismSettings settings,
            IReportLogger logger)
        {
            _settings = settings;
            _logger = logger;
        }
        
        public SimulationExchange(
            TradeMechanismSettings settings, 
            IPriceService priceService, 
            IClock clock, 
            IReportLogger logger)
        {
            _settings = settings;
            _priceService = priceService;
            _clock = clock;
            _logger = logger;
        }

        public void Initialize(EvolverSettings settings) { }

        public void Restart() { }

        public void Shutdown() { }
        public void OnTradeRequested(object? obj, TradeSubmittedEventArgs eventArgs)
        {
            if (_clock == null)
            {
                return;
            }

            DateTime time = _clock.UtcNow();
            Trade trade = eventArgs.RequestedTrade;
            var validatedTrade = Trade(time, trade, _priceService, eventArgs.AvailableFunds, _logger);
            if (validatedTrade != null)
            {
                TradeCompleted?.Invoke(null, new TradeCompletedEventArgs(trade, validatedTrade, true));
                return;
            }
                
            TradeCompleted?.Invoke(null, new TradeCompletedEventArgs(trade, null, false));
        }
        
        
        /// <inheritdoc/>
        public SecurityTrade? Trade(
            DateTime time,
            Trade trade,
            IPriceService? priceService,
            decimal availableFunds,
            IReportLogger reportLogger)
        {
            if (trade.BuySell != TradeType.Buy && trade.BuySell != TradeType.Sell)
            {
                return null;
            }

            if (priceService == null)
            {
                return null;
            }

            decimal price = trade.BuySell == TradeType.Buy
                ? priceService.GetAskPrice(time, trade.StockName)
                : priceService.GetBidPrice(time, trade.StockName);
            if (price.Equals(decimal.MinValue))
            {
                return null;
            }

            SecurityTrade tradeDetails = new SecurityTrade(
                trade.BuySell,
                trade.StockName,
                time,
                trade.NumberShares,
                price,
                _settings.TradeCost);

            if (trade.BuySell == TradeType.Buy
                && tradeDetails.TotalCost > availableFunds)
            {
                return null;
            }
            return tradeDetails;
        }
    }
}