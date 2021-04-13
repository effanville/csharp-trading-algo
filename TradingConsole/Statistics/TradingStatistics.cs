using System;
using System.Collections.Generic;
using System.IO;
using FinancialStructures.Database;
using FinancialStructures.StockStructures.Implementation;

namespace TradingConsole.Statistics
{
    public class TradingStatistics
    {
        private readonly List<TradingDaySnapshot> DayData = new List<TradingDaySnapshot>();
        private readonly List<DecisionStatistic> DecisionStats = new List<DecisionStatistic>();

        private readonly List<Trade> Transactions = new List<Trade>();

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

        public void AddDailyTrades(List<Trade> tradeDetails)
        {
            Transactions.AddRange(tradeDetails);
        }

        public void AddTrade(Trade tradeDetails)
        {
            Transactions.Add(tradeDetails);
        }

        public void AddDailyDecisionStats(DateTime day, List<string> buys, List<string> sells)
        {
            DecisionStats.Add(new DecisionStatistic(day, buys, sells));
        }

        public void AddSnapshot(DateTime day, IPortfolio portfolio)
        {
            TradingDaySnapshot snapshot = new TradingDaySnapshot();
            foreach (var security in portfolio.Funds)
            {
                snapshot.AddHolding(security.Names, security.DayData(day));
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
