using FinancialStructures.StockData;
using System;
using System.Collections.Generic;
using System.Text;

namespace TradingConsole.Statistics
{
    public class TradingStatistics
    {
        List<TradingDaySnapshot> dayData;
        List<DecisionStatistic> decisionStats;

        List<TradeDetails> transactions;

        public double StartingCash;

        public void GenerateDayStats()
        { }
        public void GenerateSimulationStats()
        { }

        public void AddTrade()
        { }
    }
}
