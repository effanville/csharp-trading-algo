using System;

using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.StockStructures;

using TradingSystem.Simulator.PriceCalculation;

namespace TradingSystem.Simulator.Trading
{
    /// <summary>
    /// Interface for holding the routine to carry out any trades.
    /// </summary>
    public interface ITradeEnactor
    {
        /// <summary>
        /// The mechanism by which the simulator will carry out trades.
        /// </summary>
        /// <param name="time">The time to enact trades at.</param>
        /// <param name="stockExchange">The stock exchange to use for available stocks.</param>
        /// <param name="portfolio">The portfolio detailing current holdings. This is updated so after the call returns the new portfolio.</param>
        /// <param name="priceCalculator">A mechanism to determine the price to trade at.</param>
        /// <param name="reportLogger">A logger to report back.</param>
        /// <returns>A record of the trades and decisions in this enaction.</returns>
        TradeEnactorResult EnactTrades(
            DateTime time,
            IStockExchange stockExchange,
            IPortfolio portfolio,
            IPriceCalculator priceCalculator,
            IReportLogger reportLogger);
    }
}
