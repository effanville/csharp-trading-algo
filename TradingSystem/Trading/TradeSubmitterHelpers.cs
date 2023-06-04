using System;

using Common.Structure.Reporting;

using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;

namespace TradingSystem.Trading
{
    public static class TradeSubmitterHelpers
    {
        public static void SubmitAndReportTrade(
            DateTime time,
            Trade trade,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            ITradeSubmitter tradeSubmitter,
            TradeHistory tradeHistory,
            TradeHistory decisionHistory,
            IReportLogger logger)
        {
            Trade validatedTrade = portfolioManager.ValidateTrade(time, trade, priceService);
            if (validatedTrade == null)
            {
                logger.Log(ReportType.Information, "Trading", $"{time} - Trade {trade} was not valid.");
                return;
            }

            decimal availableFunds = portfolioManager.AvailableFunds(time);
            if (availableFunds <= 0.0m)
            {
                logger.Log(ReportType.Information, "Trading", $"{time} - No available funds.");
                return;
            }

            var tradeConfirmation = tradeSubmitter.Trade(time, validatedTrade, priceService, availableFunds, logger);
            if (tradeConfirmation != null)
            {
                _ = portfolioManager.AddTrade(time, trade, tradeConfirmation);
                logger.Log(ReportType.Information, "Trading", $"{time} - Confirm trade '{tradeConfirmation}' reported and added.");
                tradeHistory.Add(time, validatedTrade);
            }
            decisionHistory.Add(time, trade);
        }
    }
}
