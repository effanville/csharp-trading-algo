using System;

using Common.Structure.Reporting;

using FinancialStructures.NamingStructures;

using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;

namespace TradingSystem.Trading
{
    internal static class TradeSubmitterHelpers
    {
        public static void SubmitAndReportTrade(
            DateTime time,
            Trade buy,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            ITradeMechanism tradeSubmitter,
            TradeHistory tradeHistory,
            TradeHistory decisionHistory,
            IReportLogger logger)
        {
            var actualTrade = tradeSubmitter.Trade(time, buy, priceService, portfolioManager, 0.0m, logger);
            if (actualTrade != null)
            {
                tradeHistory.Add(time, new Trade(new NameData(actualTrade.Company, actualTrade.Name), actualTrade.TradeType, actualTrade.NumberShares));
            }
            decisionHistory.Add(time, buy);
        }
    }
}
