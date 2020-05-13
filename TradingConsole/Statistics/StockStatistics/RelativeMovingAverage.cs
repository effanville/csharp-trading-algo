using FinancialStructures.StockStructures;
using StructureCommon.Mathematics;
using System;

namespace TradingConsole.Statistics
{
    public class RelativeMovingAverage : IStockStatistic
    {
        private int fFirstLength;
        private int fSecondLength;
        public int BurnInTime
        {
            get;
        }

        public StatisticType TypeOfStatistic
        {
            get;
        }

        public DataStream DataType
        {
            get;
        }

        public RelativeMovingAverage(int numberDaysOne, int numberDaysTwo, DataStream dataStream, StatisticType statisticType)
        {
            BurnInTime = Math.Max(numberDaysOne, numberDaysTwo);
            fFirstLength = numberDaysOne;
            fSecondLength = numberDaysTwo;
            TypeOfStatistic = statisticType;
            DataType = dataStream;
        }

        public double Calculate(DateTime date, Stock stock)
        {
            var values = stock.Values(date, BurnInTime, 0, DataType);
            return VectorStats.Mean(values, fFirstLength) - VectorStats.Mean(values, fSecondLength);
        }
    }
}
