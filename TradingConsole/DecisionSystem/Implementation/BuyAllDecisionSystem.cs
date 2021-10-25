using System;
using FinancialStructures.StockStructures;
using TradingConsole.Simulator;
using TradingConsole.DecisionSystem.Models;
using Common.Structure.Reporting;

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
        public void Calibrate(DecisionSystemSetupSettings decisionParameters, SimulatorSettings settings)
        {
        }

        /// <inheritdoc />
        public DecisionStatus Decide(DateTime day, SimulatorSettings settings, IReportLogger logger)
        {
            var decisions = new DecisionStatus();
            foreach (IStock stock in settings.Exchange.Stocks)
            {
                decisions.AddDecision(stock.Name, TradeDecision.Buy);
            }

            return decisions;
        }
    }
}
