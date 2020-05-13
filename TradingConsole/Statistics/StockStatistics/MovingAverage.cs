using FinancialStructures.StockStructures;
using StructureCommon.Mathematics;
using System;

namespace TradingConsole.Statistics
{
    public class MovingAverage : IStockStatistic
    {
        public DataStream DataType
        {
            get;
        }
        public int BurnInTime
        {
            get;
        }

        public StatisticType TypeOfStatistic
        {
            get;
        }

        public MovingAverage(int numberDays, DataStream dataStream, StatisticType typeOfStatistic)
        {
            BurnInTime = numberDays;
            TypeOfStatistic = typeOfStatistic;
            DataType = dataStream;
        }

        public double Calculate(DateTime date, Stock stock)
        {
            var values = stock.Values(date, BurnInTime, 0, DataType);
            return VectorStats.Mean(values, BurnInTime);
        }
    }
}
