using System;

using Common.Structure.Reporting;

using FinancialStructures.StockStructures;

using TradingSystem.DecideThenTradeSystem;
using TradingSystem.Simulator;
using TradingSystem.Simulator.Trading.Decisions;

namespace TradingConsole.DecisionSystem.Implementation
{
    /// <summary>
    /// Decision system which at any time reports to buy every stock held in the exchange.
    /// </summary>
    internal sealed class BuyAllDecisionSystem : IDecisionSystem
    {
        /// <summary>
        /// Construct and instance.
        /// </summary>
        public BuyAllDecisionSystem()
        {
        }

        /// <inheritdoc />
        public void Calibrate(StockMarketEvolver.Settings settings, IReportLogger logger)
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
