﻿using System;
using FinancialStructures.StockStructures;
using Common.Structure.Mathematics;

namespace TradingConsole.Statistics
{
    public class MovingAverage : IStockStatistic
    {
        public StockDataStream DataType
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

        public MovingAverage(int numberDays, StockDataStream dataStream, StatisticType typeOfStatistic)
        {
            BurnInTime = numberDays;
            TypeOfStatistic = typeOfStatistic;
            DataType = dataStream;
        }

        public double Calculate(DateTime date, IStock stock)
        {
            System.Collections.Generic.List<double> values = stock.Values(date, BurnInTime, 0, DataType);
            return VectorStats.Mean(values, BurnInTime);
        }
    }
}
