using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
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

            foreach (Security security in portfolio.Funds)
            {
                if (security.Value(day).Value > 0)
                {
                    security.UpdateSecurityData(stocks.GetValue(new NameData(security.GetName(), security.GetCompany()), day), new ErrorReports(), day);
                }
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
