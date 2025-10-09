using System;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    /// <summary>
    /// Decision system which at any time reports to buy every stock held in the exchange.
    /// </summary>
    internal sealed class BuyAllDecisionSystem : IDecisionSystem
    {
        public int MinBurnInPeriod => 0;

        /// <summary>
        /// Construct and instance.
        /// </summary>
        public BuyAllDecisionSystem()
        {
        }

        /// <inheritdoc />
        public void Calibrate(DecisionSystemSettings settings, IReportLogger? logger)
        {
        }

        /// <inheritdoc />
        public TradeCollection Decide(DateTime day, IStockExchange stockExchange, IReportLogger? logger)
        {
            var decisions = new TradeCollection(day, day);
            foreach (IStock stock in stockExchange.Stocks)
            {
                decisions.Add(stock.Name, TradeType.Buy);
            }

            logger?.Info(nameof(BuyAllDecisionSystem), $"Decisions: {decisions}");
            return decisions;
        }
    }
}
