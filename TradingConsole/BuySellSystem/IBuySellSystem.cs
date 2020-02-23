using FinancialStructures.Database;
using System;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    interface IBuySellSystem
    {
        void BuySell(DateTime day, DecisionStatus status, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats);
    }
}
