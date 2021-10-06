using System;
using System.Collections.Generic;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using Common.Structure.Reporting;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    internal abstract class BuySellBase : IBuySellSystem
    {
        /// <inheritdoc/>
        public IReportLogger ReportLogger
        {
            get;
        }

        internal BuySellBase(IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
        }

        public virtual void BuySell(DateTime day, DecisionStatus status, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            List<Decision> sellDecisions = status.GetSellDecisions();

            foreach (Decision sell in sellDecisions)
            {
                SellHolding(day, sell, stocks, portfolio, stats, parameters, simulationParameters);
            }

            List<Decision> buyDecisions = status.GetBuyDecisions();

            foreach (Decision buy in buyDecisions)
            {
                BuyHolding(day, buy, stocks, portfolio, stats, parameters, simulationParameters);
            }

            foreach (var security in portfolio.FundsThreadSafe)
            {
                if (security.Value(day).Value > 0)
                {
                    double value = stocks.GetValue(new NameData(security.Names.Company, security.Names.Name), day);
                    if (!value.Equals(double.NaN))
                    {
                        security.SetData(day, value, ReportLogger);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual void SellHolding(DateTime day, Decision sell, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {day} sold {sell.StockName}");
        }

        /// <inheritdoc/>
        public virtual void BuyHolding(DateTime day, Decision buy, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"Date {day} bought {buy.StockName}");
        }
    }
}
