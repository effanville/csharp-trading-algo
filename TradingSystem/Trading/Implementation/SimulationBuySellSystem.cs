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
    internal class SimulationBuySellSystem : ITradeMechanism
    {

        /// <summary>
        /// Create an instance.
        /// </summary>
        internal SimulationBuySellSystem()
        {
        }

        /// <inheritdoc/>
        public bool Buy(
            DateTime time,
            Trade buy,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            TradeMechanismSettings tradeMechanismSettings,
            IReportLogger reportLogger)
        {
            // If not a buy then stop.
            if (buy.BuySell != TradeType.Buy)
            {
                return false;
            }

            decimal cashAvailable = portfolioManager.AvailableFunds(time);
            Trade requestedTrade = portfolioManager.ValidateTrade(time, buy, priceService);
            decimal price = priceService.GetAskPrice(time, buy.StockName);
            if (price.Equals(decimal.MinValue))
            {
                return false;
            }

            if (requestedTrade == null)
            {
                return false;
            }

            buy.NumberShares = requestedTrade.NumberShares;

            // If not enough money to deal with the total cost then exit.
            SecurityTrade tradeDetails = new SecurityTrade(TradeType.Buy, buy.StockName, time, requestedTrade.NumberShares, price, tradeMechanismSettings.TradeCost);
            if (cashAvailable <= tradeDetails.TotalCost)
            {
                return false;
            }

            return portfolioManager.AddTrade(time, buy, tradeDetails);
        }

        /// <inheritdoc/>
        public bool Sell(
            DateTime time,
            Trade sell,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            TradeMechanismSettings tradeMechanismSettings,
            IReportLogger reportLogger)

        {
            if (sell.BuySell != TradeType.Sell)
            {
                return false;
            }

            Trade requestedTrade = portfolioManager.ValidateTrade(time, sell, priceService);

            decimal price = priceService.GetBidPrice(time, sell.StockName);
            if (requestedTrade == null)
            {
                return false;
            }

            // some error with price data (or shouldnt be evaluating on this date) so ignore trade.
            if (price.Equals(decimal.MinValue))
            {
                return false;
            }

            sell.NumberShares = requestedTrade.NumberShares;
            SecurityTrade tradeDetails = new SecurityTrade(TradeType.Sell, requestedTrade.StockName, time, requestedTrade.NumberShares, price, tradeMechanismSettings.TradeCost);
            return portfolioManager.AddTrade(time, sell, tradeDetails);
        }

        /// <inheritdoc/>
        public TradeCollection EnactAllTrades(
            DateTime time,
            TradeCollection decisions,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            TradeMechanismSettings tradeMechanismSettings,
            IReportLogger reportLogger)
        {
            return this.SellThenBuy(time, decisions, priceService, portfolioManager, tradeMechanismSettings, reportLogger);
        }
    }
}
