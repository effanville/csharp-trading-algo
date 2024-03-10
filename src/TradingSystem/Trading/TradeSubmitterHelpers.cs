using System;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingSystem.Trading
{
    public static class TradeSubmitterHelpers
    {
        public static void SubmitAndReportTrade(
            DateTime time,
            Trade trade,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            IMarketExchange tradeSubmitter,
            TradeHistory tradeHistory,
            TradeHistory decisionHistory,
            IReportLogger logger)
        {
            Trade? validatedTrade = portfolioManager.ValidateTrade(time, trade, priceService);
            if (validatedTrade == null)
            {
                logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Trade {trade} was not valid.");
                return;
            }

            decimal availableFunds = portfolioManager.AvailableFunds(time);
            if (availableFunds <= 0.0m)
            {
                logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - No available funds.");
                return;
            }

            var tradeConfirmation = tradeSubmitter.Trade(time, validatedTrade, priceService, availableFunds, logger);
            if (tradeConfirmation != null)
            {
                logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Confirm trade '{tradeConfirmation}' reported and added.");
                _ = portfolioManager.AddTrade(time, trade, tradeConfirmation);
                tradeHistory.Add(time, validatedTrade);
            }
            decisionHistory.Add(time, trade);
        }
    }
}
