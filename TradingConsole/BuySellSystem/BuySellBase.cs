using System;
using FinancialStructures.Database;
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
        public IReportLogger ReportLogger
        {
            get;
        }

        public BuySellBase(IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
        }

        public virtual void BuySell(DateTime day, DecisionStatus status, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
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

            foreach (var security in portfolio.Funds)
            {
                if (security.Value(day).Value > 0)
                {
                    var value = stocks.GetValue(new NameData(security.Company, security.Name), day);
                    if (!value.Equals(double.NaN))
                    {
                        security.TryAddOrEditData(day, day, value, ReportLogger);
                    }
                }
            }
        }

        public virtual void SellHolding(DateTime day, Decision sell, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
        }

        public virtual void BuyHolding(DateTime day, Decision buy, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
        }
    }
}
