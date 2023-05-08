using System;

using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using TradingSystem.DecideThenTradeSystem;

namespace TradingSystem.Trading
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
            Trade buy,
            Func<DateTime, TwoName, decimal> calculateBuyPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);

        /// <summary>
        /// Enact a sell decision
        /// </summary>
        bool Sell(
            DateTime time,
            Trade sell,
            Func<DateTime, TwoName, decimal> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);

        /// <summary>
        /// Routine to enact all trades.
        /// </summary>
        TradeCollection EnactAllTrades(
            DateTime time,
            TradeCollection decisions,
            Func<DateTime, TwoName, decimal> calculateBuyPrice,
            Func<DateTime, TwoName, decimal> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);
    }
}
