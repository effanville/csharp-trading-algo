using System;
using FinancialStructures.Database;
using FinancialStructures.StockStructures;
using TradingConsole.DecisionSystem.Models;

namespace TradingConsole.BuySellSystem
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
            IStockExchange exchange,
            IPortfolio portfolio,
            TradeMechanismSettings settings,
            TradeMechanismTraderOptions traderOptions);

        /// <summary>
        /// Enact a sell decision
        /// </summary>
        bool Sell(
            DateTime time,
            Decision sell,
            IStockExchange exchange,
            IPortfolio portfolio,
            TradeMechanismSettings settings,
            TradeMechanismTraderOptions traderOptions);
    }
}
