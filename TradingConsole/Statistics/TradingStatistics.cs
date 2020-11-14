using System;
using System.Collections.Generic;
using System.IO;
using FinancialStructures.Database;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockData;

namespace TradingConsole.Statistics
{
    public class TradingStatistics
    {
        private readonly List<TradingDaySnapshot> DayData = new List<TradingDaySnapshot>();
        private readonly List<DecisionStatistic> DecisionStats = new List<DecisionStatistic>();

        private readonly List<TradeDetails> Transactions = new List<TradeDetails>();

        public double StartingCash;

        /// <summary>
        /// Still to be implemented.
        /// </summary>
        public void GenerateDayStats()
        {
        }

        public void GenerateSimulationStats()
        {
        }

        public void AddDailyTrades(List<TradeDetails> tradeDetails)
        {
            Transactions.AddRange(tradeDetails);
        }

        public void AddTrade(TradeDetails tradeDetails)
        {
            Transactions.Add(tradeDetails);
        }

        public void AddDailyDecisionStats(DateTime day, List<string> buys, List<string> sells)
        {
            DecisionStats.Add(new DecisionStatistic(day, buys, sells));
        }

        public void AddSnapshot(DateTime day, Portfolio portfolio)
        {
            TradingDaySnapshot snapshot = new TradingDaySnapshot();
            foreach (FinancialStructures.FinanceStructures.Security security in portfolio.Funds)
            {
                snapshot.AddHolding(new NameData(security.Company, security.Name), security.DayData(day));
            }
            snapshot.Time = day;
            snapshot.freeCash = portfolio.TotalValue(Totals.BankAccount, day);
            snapshot.TotalHoldingValue = portfolio.TotalValue(Totals.Security, day);
            DayData.Add(snapshot);
        }

        public void ExportToFile(string filePath)
        {
            try
            {
                StreamWriter writer = new StreamWriter(filePath);
                writer.WriteLine("Trades:");
                foreach (var trade in Transactions)
                {
                    writer.WriteLine(trade.ToString());
                }

                writer.WriteLine("History of Portfolio:");
                foreach (var day in DayData)
                {
                    writer.WriteLine(day.ToString());
                }

                writer.Close();
            }
            catch (IOException exception)
            {
            }
        }
    }
}
