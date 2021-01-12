using System;
using FinancialStructures.StockStructures;
using StructureCommon.Mathematics;

namespace TradingConsole.Statistics
{
    public class RelativeMovingAverage : IStockStatistic
    {
        private readonly int fFirstLength;
        private readonly int fSecondLength;
        public int BurnInTime
        {
            get;
        }

        public StatisticType TypeOfStatistic
        {
            get;
        }

        public StockDataStream DataType
        {
            get;
        }

        public RelativeMovingAverage(int numberDaysOne, int numberDaysTwo, StockDataStream dataStream, StatisticType statisticType)
        {
            BurnInTime = Math.Max(numberDaysOne, numberDaysTwo);
            fFirstLength = numberDaysOne;
            fSecondLength = numberDaysTwo;
            TypeOfStatistic = statisticType;
            DataType = dataStream;
        }

        public double Calculate(DateTime date, Stock stock)
        {
            System.Collections.Generic.List<double> values = stock.Values(date, BurnInTime, 0, DataType);
            return VectorStats.Mean(values, fFirstLength) - VectorStats.Mean(values, fSecondLength);
        }
    }
}
