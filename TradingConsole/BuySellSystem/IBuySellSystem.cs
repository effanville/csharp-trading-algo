using FinancialStructures.Database;
using FinancialStructures.ReportLogging;
using FinancialStructures.StockStructures;
using System;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    interface IBuySellSystem
    {
        LogReporter ReportLogger { get; }
        void BuySell(DateTime day, DecisionStatus status, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);

        void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);

        void SellHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);
    }
}
