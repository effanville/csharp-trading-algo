using System;
using Common.Structure.Reporting;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using TradingSystem.Decisions.Models;
using TradingSystem.Trading.Models;

namespace TradingSystem.Trading.System
{
    /// <summary>
    /// Mechanism to enact the buying and selling of stocks.
    /// e.g. one could have a simulation system, or one could use this to interact with a broker.
    /// </summary>
    public interface ITradeMechanism
    {
        /// <summary>
        /// Enact a buy decision.
        /// </summary>
        bool Buy(
            DateTime time,
            Decision buy,
            Func<DateTime, NameData, double> calculateBuyPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);

        /// <summary>
        /// Enact a sell decision
        /// </summary>
        bool Sell(
            DateTime time,
            Decision sell,
            Func<DateTime, NameData, double> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);

        /// <summary>
        /// Routine to enact all trades.
        /// </summary>
        TradeStatus EnactAllTrades(
            DateTime time,
            DecisionStatus decisions,
            Func<DateTime, NameData, double> calculateBuyPrice,
            Func<DateTime, NameData, double> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);
    }
}
