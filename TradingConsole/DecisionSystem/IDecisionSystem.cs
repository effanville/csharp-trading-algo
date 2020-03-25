using FinancialStructures.ReportLogging;
using System;
using System.Collections.Generic;
using TradingConsole.Simulation;
using TradingConsole.Statistics;
using TradingConsole.StockStructures;

namespace TradingConsole.DecisionSystem
{
    interface IDecisionSystem
    {
        LogReporter ReportLogger { get; }
        void Decide(DateTime day, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats, SimulationParameters simulationParameters);

        void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells);
    }
}
