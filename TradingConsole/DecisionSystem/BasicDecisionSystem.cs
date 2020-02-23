using System;
using TradingConsole.Statistics;
using TradingConsole.StockStructures;

namespace TradingConsole.DecisionSystem
{
    public class BasicDecisionSystem : IDecisionSystem
    {
        public void Decide(DateTime date, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats)
        {
            foreach (var stock in exchange.Stocks)
            {
                status.AddDecision(stock.Name, StockTradeDecision.Buy);
            }
        }
    }
}
