using System;
using System.Collections.Generic;
using TradingConsole.Statistics;

namespace TradingConsole.InputParser
{
    /// <summary>
    /// Holds the inputs the user specifies as arguments for the program
    /// </summary>
    public class UserInputOptions
    {
        public ProgramType funtionType;
        public string StockFilePath;
        public string PortfolioFilePath;
        public double StartingCash;

        public DateTime StartDate;
        public DateTime EndDate;
        public TimeSpan TradingGap;
        public DecisionSystemType DecisionType;
        public List<StatisticType> decisionSystemStats = new List<StatisticType>();
        public BuySellType BuyingSellingType;
    }
}
