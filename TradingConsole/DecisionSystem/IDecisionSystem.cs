using FinancialStructures.ReportLogging;
using FinancialStructures.StockStructures;
using System;
using System.Collections.Generic;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    interface IDecisionSystem
    {
        LogReporter ReportLogger { get; }
        void Decide(DateTime day, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats, SimulationParameters simulationParameters);

        void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells);
    }
}
