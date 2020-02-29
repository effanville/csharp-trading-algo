using FinancialStructures.Database;
using System;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;
using TradingConsole.StockStructures;

namespace TradingConsole.BuySellSystem
{
    public class BuySellBase : IBuySellSystem
    {
        public virtual void BuySell(DateTime day, DecisionStatus status, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            var sellDecisions = status.GetSellDecisions();

            foreach (var sell in sellDecisions)
            {
                SellHolding(day, sell, stocks, portfolio, stats, parameters, simulationParameters);
            }

            var buyDecisions = status.GetBuyDecisions();

            foreach (var buy in buyDecisions)
            {
                BuyHolding(day, buy, stocks, portfolio, stats, parameters, simulationParameters);
            }
        }

        public virtual void SellHolding(DateTime day, Decision sell, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
        }

        public virtual void BuyHolding(DateTime day, Decision buy, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
        }
    }
}
