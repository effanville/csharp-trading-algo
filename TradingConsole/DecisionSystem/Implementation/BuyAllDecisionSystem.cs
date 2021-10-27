using System;

using Common.Structure.Reporting;

using FinancialStructures.StockStructures;

using TradingSystem.Decisions.System;
using TradingSystem.Simulator;
using TradingSystem.Decisions.Models;

namespace TradingConsole.DecisionSystem.Implementation
{
    /// <summary>
    /// Decision system which at any time reports to buy every stock held in the exchange.
    /// </summary>
    public class BuyAllDecisionSystem : IDecisionSystem
    {
        /// <summary>
        /// Construct and instance.
        /// </summary>
        public BuyAllDecisionSystem()
        {
        }

        /// <inheritdoc />
        public void Calibrate(SimulatorSettings settings, IReportLogger logger)
        {
        }

        /// <inheritdoc />
        public DecisionStatus Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            var decisions = new DecisionStatus();
            foreach (IStock stock in stockExchange.Stocks)
            {
                decisions.AddDecision(stock.Name, TradeDecision.Buy);
            }

            return decisions;
        }
    }
}
