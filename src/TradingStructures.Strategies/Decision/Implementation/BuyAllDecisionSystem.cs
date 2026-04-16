using System;

using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Trading;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    /// <summary>
    /// Decision system which at any time reports to buy every stock held in the exchange.
    /// </summary>
    internal sealed class BuyAllDecisionSystem : IDecisionSystem
    {
        private readonly ILogger<BuyAllDecisionSystem> _logger;

        public BuyAllDecisionSystem(ILogger<BuyAllDecisionSystem> logger)
        {
            _logger = logger;
        }

        public int MinBurnInPeriod => 0;

        /// <inheritdoc />
        public void Calibrate(DecisionSystemSettings settings)
        {
        }

        /// <inheritdoc />
        public TradeCollection Decide(DateTime day, IStockExchange stockExchange)
        {
            var decisions = new TradeCollection(day, day);
            foreach (IStock stock in stockExchange.Stocks)
            {
                decisions.Add(stock.Name, TradeType.Buy);
            }

            _logger.LogInformation($"Decisions: {decisions}");
            return decisions;
        }
    }
}
