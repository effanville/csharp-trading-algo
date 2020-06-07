using System;
using FinancialStructures.StockStructures;
using StructureCommon.Mathematics;

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
            System.Collections.Generic.List<double> values = stock.Values(date, BurnInTime, 0, DataType);
            return VectorStats.Mean(values, BurnInTime);
        }
    }
}
