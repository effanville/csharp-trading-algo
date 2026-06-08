using System;

using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common;

using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.MarketData;

namespace Effanville.TradingStructures.StockMarket.Implementation
{
    internal class SimulationExchange : IStockMarketAdapter
    {
        private readonly IClock _clock;
        private readonly IPriceService _priceService;
        private readonly StockMarketAdapterSettings _settings;

        public event EventHandler<TradeResponseEventArgs>? TradeCompleted;

        public string Name => nameof(SimulationExchange);

        public SimulationExchange(
            StockMarketAdapterSettings settings,
            IPriceService priceService,
            IClock clock)
        {
            _settings = settings;
            _priceService = priceService;
            _clock = clock;
        }

        public void Initialize(EvolverSettings settings) { }

        public void OnTradeRequested(object? obj, TradeRequestEventArgs eventArgs)
        {
            if (_clock == null)
            {
                return;
            }

            DateTime time = _clock.UtcNow();
            Trade trade = eventArgs.RequestedTrade;
            var validatedTrade = Trade(time, trade, _priceService);
            if (validatedTrade != null)
            {
                TradeCompleted?.Invoke(null, new TradeResponseEventArgs(eventArgs.Id, trade, validatedTrade, true));
                return;
            }

            TradeCompleted?.Invoke(null, new TradeResponseEventArgs(eventArgs.Id, trade, null, false));
        }

        private SecurityTrade? Trade(
            DateTime time,
            Trade trade,
            IPriceService? priceService)


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
            return tradeDetails;
        }
    }
}
