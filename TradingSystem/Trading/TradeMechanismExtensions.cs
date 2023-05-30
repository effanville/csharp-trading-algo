using System;
using System.Collections.Generic;

using Common.Structure.Reporting;

using FinancialStructures.Database;

using TradingSystem.DecideThenTradeSystem;
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
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger)
        {
            List<Trade> sellDecisions = decisions.GetSellDecisions();
            var trades = new TradeCollection(time, time);
            bool wasTrade = false;
            foreach (Trade sell in sellDecisions)
            {
                if (tradeMechanism.Sell(time, sell, priceService, portfolio, traderOptions, reportLogger))
                {
                    wasTrade = true;
                    trades.Add(sell.StockName, sell.BuySell, sell.NumberShares);
                }
            }

            List<Trade> buyDecisions = decisions.GetBuyDecisions();
            foreach (Trade buy in buyDecisions)
            {
                if (tradeMechanism.Buy(time, buy, priceService, portfolio, traderOptions, reportLogger))
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
