using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
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
            foreach (var security in portfolio.GetSecurities())
            {
                snapshot.AddHolding(new NameData(security.GetName(), security.GetCompany()), security.DayData(day));
            }

            snapshot.freeCash = portfolio.AllBankAccountsValue(day);
            DayData.Add(snapshot);
        }
    }
}
