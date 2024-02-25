using System;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.DataStructures;

using TradingSystem.MarketEvolvers;
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

        public string Name => nameof(SimulationBuySellSystem);

        /// <summary>
        /// Create an instance.
        /// </summary>
        internal SimulationBuySellSystem(TradeMechanismSettings settings)
        {
            Settings = settings;
        }

        public void Initialize(EvolverSettings settings) { }
        public void Restart() => throw new NotImplementedException();
        public void Shutdown() { }

        /// <inheritdoc/>
        public SecurityTrade Trade(
            DateTime time,
            Trade trade,
            IPriceService priceService,
            decimal availableFunds,
            IReportLogger reportLogger)
        {
            if (trade.BuySell != TradeType.Buy && trade.BuySell != TradeType.Sell)
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
                Settings.TradeCost);

            if (trade.BuySell == TradeType.Buy
                && tradeDetails.TotalCost > availableFunds)
            {
                return null;
            }
            return tradeDetails;
        }
    }
}
