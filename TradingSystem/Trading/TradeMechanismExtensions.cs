using System;
using System.Collections.Generic;

using Common.Structure.Reporting;

using TradingSystem.DecideThenTradeSystem;
using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;

namespace TradingSystem.Trading
{
    public static class TradeMechanismExtensions
    {
        public static TradeCollection SellThenBuy(
            this ITradeMechanism tradeMechanism,
            DateTime time,
            TradeCollection decisions,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger)
        {
            List<Trade> sellDecisions = decisions.GetSellDecisions();
            var trades = new TradeCollection(time, time);
            bool wasTrade = false;
            foreach (Trade sell in sellDecisions)
            {
                if (tradeMechanism.Sell(time, sell, priceService, portfolioManager, traderOptions, reportLogger))
                {
                    wasTrade = true;
                    trades.Add(sell.StockName, sell.BuySell, sell.NumberShares);
                }
            }

            List<Trade> buyDecisions = decisions.GetBuyDecisions();
            foreach (Trade buy in buyDecisions)
            {
                if (tradeMechanism.Buy(time, buy, priceService, portfolioManager, traderOptions, reportLogger))
                {
                    wasTrade = true;
                    trades.Add(buy.StockName, buy.BuySell, buy.NumberShares);
                }
            }

            if (!wasTrade)
            {
                return null;
            }

            return trades;
        }
    }
}
