using FinancialStructures.StockStructures;
using System;
using System.Linq;

namespace TradingConsole.Statistics
{
    public class PreviousNDayValue : IStockStatistic
    {
        private int N;
        private DataStream DataType;
        private StatisticType fTypeOfStatistic;

        public PreviousNDayValue(int n, DataStream dataType, StatisticType typeOfStatistic)
        {
            N = n;
            DataType = dataType;
            fTypeOfStatistic = typeOfStatistic;
        }

        public StatisticType TypeOfStatistic => fTypeOfStatistic;

        public double Calculate(DateTime date, Stock stock)
        {
            return stock.Values(date, N, 0, DataType).First();
        }
    }
}
