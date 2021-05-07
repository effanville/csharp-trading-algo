using System;
using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using StructureCommon.DataStructures;
using StructureCommon.Reporting;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    internal class IBClientTradingSystem : BuySellBase
    {
        internal IBClientTradingSystem(IReportLogger reportLogger)
            : base(reportLogger)
        {
        }

        /// <inheritdoc/>
        public override void SellHolding(DateTime day, Decision sell, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, day);
            double price = stocks.GetValue(sell.StockName, day);
            double numShares = portfolio.SecurityPrices(sell.StockName, day, SecurityDataStream.NumberOfShares);
            var trade = new SecurityTrade(TradeType.Sell, sell.StockName, day, numShares, price, simulationParameters.TradeCost);
            _ = portfolio.TryAddOrEditDataToSecurity(sell.StockName, day, day, 0.0, price, 1.0, trade, ReportLogger);
            var data = new DailyValuation(day, cashAvailable + trade.TotalCost);
            _ = portfolio.TryAddOrEditData(Account.BankAccount, simulationParameters.BankAccData, data, data, ReportLogger);
            base.SellHolding(day, sell, stocks, portfolio, stats, parameters, simulationParameters);
        }

        /// <inheritdoc/>
        public override void BuyHolding(DateTime day, Decision buy, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters)
        {
            double price = stocks.GetValue(buy.StockName, day);
            double cashAvailable = portfolio.TotalValue(Totals.BankAccount, day);
            if (price != 0)
            {
                int numShares = 0;
                while (numShares * price < parameters.fractionInvest)
                {
                    numShares++;
                }
                numShares--;

                var trade = new SecurityTrade(TradeType.Buy, buy.StockName, day, numShares, price, simulationParameters.TradeCost);
                if (cashAvailable > trade.TotalCost)
                {
                    _ = portfolio.TryAddOrEditDataToSecurity(buy.StockName, day, day, numShares, price, 1, trade, ReportLogger);
                    var data = new DailyValuation(day, cashAvailable - trade.TotalCost);
                    _ = portfolio.TryAddOrEditData(Account.BankAccount, new NameData("Cash", "Portfolio"), data, data, ReportLogger);
                }
            }

            base.BuyHolding(day, buy, stocks, portfolio, stats, parameters, simulationParameters);
        }
    }
}

