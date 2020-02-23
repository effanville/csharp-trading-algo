using System;
using TradingConsole.Statistics;

namespace TradingConsole. DecisionSystem
{
    public class BuyAllDecisionSystem : IDecisionSystem
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
