﻿using System;
using FinancialStructures.StockStructures;

namespace TradingConsole.Statistics
{
    public class StochasticStat : IStockStatistic
    {
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
            get
            {
                return StockDataStream.None;
            }
        }

        public StochasticStat(int numberDays, StatisticType statisticType)
        {
            BurnInTime = numberDays;
            TypeOfStatistic = statisticType;
        }

        public double Calculate(DateTime date, IStock stock)
        {
            return stock.Stochastic(date, BurnInTime);
        }
    }
}
