using System;

namespace TradingConsole.InputParser
{
    public class UserInputOptions
    {
        public int StartTime;
        public DateTime StartDate;
        public int EndTime;
        public DateTime EndDate;

        public TimeSpan TradingGap;

        public string StockFilePath;
        public string PortfolioFilePath;

        public double StartingCash;

        public FunctionType funtionType;
    }
}
