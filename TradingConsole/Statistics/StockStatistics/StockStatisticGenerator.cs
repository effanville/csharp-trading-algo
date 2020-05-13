﻿using FinancialStructures.StockStructures;

namespace TradingConsole.Statistics
{
    /// <summary>
    /// Generator for various statistic calculators for a stock.
    /// </summary>
    public static class StockStatisticGenerator
    {
        /// <summary>
        /// Generates a statistic calculator of the type specified.
        /// </summary>
        public static IStockStatistic Generate(StatisticType typeStatistic)
        {
            switch (typeStatistic)
            {
                case StatisticType.TodayOpen:
                    return new PreviousNDayValue(0, DataStream.Open, typeStatistic);
                case StatisticType.PrevTwoOpen:
                    return new PreviousNDayValue(2, DataStream.Open, typeStatistic);
                case StatisticType.PrevThreeOpen:
                    return new PreviousNDayValue(3, DataStream.Open, typeStatistic);
                case StatisticType.PrevFourOpen:
                    return new PreviousNDayValue(4, DataStream.Open, typeStatistic);
                case StatisticType.PrevFiveOpen:
                    return new PreviousNDayValue(5, DataStream.Open, typeStatistic);
                case StatisticType.TodayClose:
                    return new PreviousNDayValue(0, DataStream.Close, typeStatistic);
                case StatisticType.PrevDayClose:
                    return new PreviousNDayValue(1, DataStream.Close, typeStatistic);
                case StatisticType.PrevTwoClose:
                    return new PreviousNDayValue(2, DataStream.Close, typeStatistic);
                case StatisticType.PrevThreeClose:
                    return new PreviousNDayValue(3, DataStream.Close, typeStatistic);
                case StatisticType.PrevFourClose:
                    return new PreviousNDayValue(4, DataStream.Close, typeStatistic);
                case StatisticType.PrevFiveClose:
                    return new PreviousNDayValue(5, DataStream.Close, typeStatistic);
                case StatisticType.PrevDayHigh:
                    return new PreviousNDayValue(1, DataStream.High, typeStatistic);
                case StatisticType.PrevDayLow:
                    return new PreviousNDayValue(1, DataStream.Low, typeStatistic);
                case StatisticType.MovingAverageWeek:
                    return new MovingAverage(5, DataStream.Open, typeStatistic);
                case StatisticType.MovingAverageMonth:
                    return new MovingAverage(20, DataStream.Open, typeStatistic);
                case StatisticType.MovingAverageHalfYear:
                    return new MovingAverage(50, DataStream.Open, typeStatistic);
                case StatisticType.RelativeMovingAverageWeekMonth:
                    return new RelativeMovingAverage(5, 20, DataStream.Open, typeStatistic);
                case StatisticType.RelativeMovingAverageMonthHalfYear:
                    return new RelativeMovingAverage(20, 50, DataStream.Open, typeStatistic);
                case StatisticType.RelativeMovingAverageWeekHalfYear:
                    return new RelativeMovingAverage(5, 50, DataStream.Open, typeStatistic);
                case StatisticType.ADXStatTwoWeek:
                    return new ADXStat(14, typeStatistic);
                case StatisticType.StochasticTwoWeek:
                    return new StochasticStat(14, typeStatistic);
                case StatisticType.StochasticFourWeek:
                    return new StochasticStat(28, typeStatistic);
                case StatisticType.PrevDayOpen:
                default:
                    return new PreviousDayOpen();
            }
        }
    }
}