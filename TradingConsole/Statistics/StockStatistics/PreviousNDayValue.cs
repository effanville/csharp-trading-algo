using System;
using System.Linq;
using FinancialStructures.StockStructures;

namespace TradingConsole.Statistics
{
    public class PreviousNDayValue : IStockStatistic
    {
        public StockDataStream DataType
        {
            get;
        }

        public StatisticType TypeOfStatistic
        {
            get;
        }

        public int BurnInTime
        {
            get;
        }

        public PreviousNDayValue(int n, StockDataStream dataType, StatisticType typeOfStatistic)
        {
            BurnInTime = n;
            DataType = dataType;
            TypeOfStatistic = typeOfStatistic;
        }

        public double Calculate(DateTime date, Stock stock)
        {
            return stock.Values(date, BurnInTime, 0, DataType).First();
        }
    }
}
