using System;
using System.Collections.Generic;

using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using TradingSystem.DecideThenTradeSystem;

namespace TradingSystem.Trading
{
    public static class TradeMechanismExtensions
    {
        public static TradeCollection SellThenBuy(
            this ITradeMechanism tradeMechanism,
            DateTime time,
            TradeCollection decisions,
            Func<DateTime, TwoName, decimal> calculateBuyPrice,
            Func<DateTime, TwoName, decimal> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger)
        {
            List<Trade> sellDecisions = decisions.GetSellDecisions();
            var trades = new TradeCollection(time, time);
            bool wasTrade = false;
            foreach (Trade sell in sellDecisions)
            {
                if (tradeMechanism.Sell(time, sell, calculateSellPrice, portfolio, traderOptions, reportLogger))
                {
                    wasTrade = true;
                    trades.Add(sell.StockName, sell.BuySell, sell.NumberShares);
                }
            }

            List<Trade> buyDecisions = decisions.GetBuyDecisions();
            foreach (Trade buy in buyDecisions)
            {
                if (tradeMechanism.Buy(time, buy, calculateBuyPrice, portfolio, traderOptions, reportLogger))
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
