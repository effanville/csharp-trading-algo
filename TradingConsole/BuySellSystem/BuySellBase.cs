using System;
using FinancialStructures.Database;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using StructureCommon.Reporting;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    public class BuySellBase : IBuySellSystem
    {
        public LogReporter ReportLogger
        {
            get;
        }

        public BuySellBase(LogReporter reportLogger)
        {
            ReportLogger = reportLogger;
        }

        public virtual void BuySell(DateTime day, DecisionStatus status, ExchangeStocks stocks, Portfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            System.Collections.Generic.List<Decision> sellDecisions = status.GetSellDecisions();

            foreach (Decision sell in sellDecisions)
            {
                SellHolding(day, sell, stocks, portfolio, stats, parameters, simulationParameters);
            }

            System.Collections.Generic.List<Decision> buyDecisions = status.GetBuyDecisions();

            foreach (Decision buy in buyDecisions)
            {
                BuyHolding(day, buy, stocks, portfolio, stats, parameters, simulationParameters);
            }

            foreach (ISecurity security in portfolio.Funds)
            {
                if (security.Value(day).Value > 0)
                {
                    security.UpdateSecurityData(day, stocks.GetValue(new NameData(security.Company, security.Name), day), ReportLogger);
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
