using System;
using System.Collections.Generic;

using Common.Structure.Reporting;

using FinancialStructures.NamingStructures;

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
            IReportLogger reportLogger)
        {
            List<Trade> sellDecisions = decisions.GetSellDecisions();
            var trades = new TradeCollection(time, time);
            bool wasTrade = false;
            foreach (Trade sell in sellDecisions)
            {
                var actualTrade = tradeMechanism.Trade(time, sell, priceService, portfolioManager, 0.0m, reportLogger);
                if (actualTrade != null)
                {
                    wasTrade = true;
                    trades.Add(new NameData(actualTrade.Company, actualTrade.Name), actualTrade.TradeType, actualTrade.NumberShares);
                }
            }

            List<Trade> buyDecisions = decisions.GetBuyDecisions();
            foreach (Trade buy in buyDecisions)
            {
                var actualTrade = tradeMechanism.Trade(time, buy, priceService, portfolioManager, 0.0m, reportLogger);
                if (actualTrade != null)
                {
                    wasTrade = true;
                    trades.Add(new NameData(actualTrade.Company, actualTrade.Name), actualTrade.TradeType, actualTrade.NumberShares);
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
