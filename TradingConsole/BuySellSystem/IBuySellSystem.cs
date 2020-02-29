using FinancialStructures.Database;
using System;
using TradingConsole.Statistics;
using TradingConsole.DecisionSystem;
using TradingConsole.StockStructures;
using TradingConsole.Simulation;

namespace TradingConsole.BuySellSystem
{
    interface IBuySellSystem
    {
        void BuySell(DateTime day, DecisionStatus status, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);

        void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);

        void SellHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);
    }
}
