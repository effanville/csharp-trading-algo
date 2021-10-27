using System;

using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

using TradingSystem.Decisions.Models;
using TradingSystem.Trading.Models;

namespace TradingSystem.Simulator
{
    /// <summary>
    /// Interface for holding the routine to carry out any trades.
    /// </summary>
    public interface ITradeEnactor
    {
        /// <summary>
        /// Carry out trades.
        /// </summary>
        /// <param name="time">The time to enact trades at.</param>
        /// <param name="stockExchange">The stock exchange to use for available stocks.</param>
        /// <param name="portfolio">The portfolio detailing current holdings. This is updated so after the call returns the new portfolio.</param>
        /// <param name="calcBuyPrice">A function that calculates how a price for a buy trade is carried out.</param>
        /// <param name="calcSellPrice">A function that calculates how a price for a sell trade is carried out.</param>
        /// <param name="reportLogger">A logger to report back.</param>
        /// <returns>A record of the trades and decisions in this enaction.</returns>
        (TradeStatus Trades, DecisionStatus Decisions) EnactTrades(
            DateTime time,
            IStockExchange stockExchange,
            IPortfolio portfolio,
            Func<DateTime, TwoName, double> calcBuyPrice,
            Func<DateTime, TwoName, double> calcSellPrice,
            IReportLogger reportLogger);
    }
}
