using System;

using Common.Structure.Reporting;

using FinancialStructures.DataStructures;

using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;

namespace TradingSystem.Trading.Implementation
{
    /// <summary>
    /// Trading system for use in simulation systems.
    /// </summary>
    internal class SimulationBuySellSystem : ITradeSubmitter
    {

        /// <inheritdoc/>
        public TradeMechanismSettings Settings { get; }

        /// <summary>
        /// Create an instance.
        /// </summary>
        internal SimulationBuySellSystem(TradeMechanismSettings settings)
        {
            Settings = settings;
        }

        /// <inheritdoc/>
        public SecurityTrade Trade(
            DateTime time,
            Trade trade,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            decimal availableFunds,
            IReportLogger reportLogger)
        {
            if (trade.BuySell != TradeType.Buy && trade.BuySell != TradeType.Sell)
            {
                return null;
            }

            availableFunds = portfolioManager.AvailableFunds(time);
            Trade requestedTrade = portfolioManager.ValidateTrade(time, trade, priceService);
            decimal price = trade.BuySell == TradeType.Buy
                ? priceService.GetAskPrice(time, trade.StockName)
                : priceService.GetBidPrice(time, trade.StockName);
            if (price.Equals(decimal.MinValue))
            {
                return null;
            }

            if (requestedTrade == null)
            {
                return null;
            }

            SecurityTrade tradeDetails = new SecurityTrade(
                requestedTrade.BuySell,
                requestedTrade.StockName,
                time,
                requestedTrade.NumberShares,
                price,
                Settings.TradeCost);

            if (requestedTrade.BuySell == TradeType.Buy
                && tradeDetails.TotalCost > availableFunds)
            {
                return null;
            }
            portfolioManager.AddTrade(time, requestedTrade, tradeDetails);
            return tradeDetails;
        }
    }
}
