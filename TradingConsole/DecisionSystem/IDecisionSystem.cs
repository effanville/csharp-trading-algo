using System;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    interface IDecisionSystem
    {
        void Decide(DateTime day, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats);
    }
}
