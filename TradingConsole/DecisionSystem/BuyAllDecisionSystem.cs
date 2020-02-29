using System;
using System.Collections.Generic;
using TradingConsole.Simulation;
using TradingConsole.Statistics;
using TradingConsole.StockStructures;

namespace TradingConsole. DecisionSystem
{
    public class BuyAllDecisionSystem : IDecisionSystem
    {
        public void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells)
        {
            stats.AddDailyDecisionStats(day, buys, sells);
        }

        public void Decide(DateTime date, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats, SimulationParameters simulationParameters)
        {
            foreach (var stock in exchange.Stocks)
            {
                status.AddDecision(stock.Name, StockTradeDecision.Buy);
            }
        }
    }
}
