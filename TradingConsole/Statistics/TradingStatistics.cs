using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.StockData;
using System;
using System.Collections.Generic;

namespace TradingConsole.Statistics
{
    public class TradingStatistics
    {
        private List<TradingDaySnapshot> DayData = new List<TradingDaySnapshot>();
        private List<DecisionStatistic> DecisionStats = new List<DecisionStatistic>();

        private List<TradeDetails> Transactions = new List<TradeDetails>();

        public double StartingCash;

        /// <summary>
        /// Still to be implemented.
        /// </summary>
        public void GenerateDayStats()
        { }

        public void GenerateSimulationStats()
        { }

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
            var snapshot = new TradingDaySnapshot();
            foreach (var security in portfolio.Funds)
            {
                snapshot.AddHolding(new NameData(security.Company, security.Name), security.DayData(day));
            }

            snapshot.freeCash = portfolio.TotalValue(AccountType.BankAccount, day);
            DayData.Add(snapshot);
        }
    }
}
