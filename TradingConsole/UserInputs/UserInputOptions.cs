using System;

namespace TradingConsole.InputParser
{
    public class UserInputOptions
    {
        public DateTime StartDate;
        public DateTime EndDate;

        public TimeSpan TradingGap;

        public string StockFilePath;
        public string PortfolioFilePath;

        public double StartingCash;

        public FunctionType funtionType;
    }
}
