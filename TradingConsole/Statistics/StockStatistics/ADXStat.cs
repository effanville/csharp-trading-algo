using FinancialStructures.StockStructures;
using System;

namespace TradingConsole.Statistics
{
    public class ADXStat : IStockStatistic
    {
        public int BurnInTime { get; }

        public StatisticType TypeOfStatistic { get; }

        public DataStream DataType => DataStream.None;

        public ADXStat(int numberDays, StatisticType statisticType)
        {
            BurnInTime = numberDays;
            TypeOfStatistic = statisticType;
        }

        public double Calculate(DateTime date, Stock stock)
        {
            return stock.ADX(date, BurnInTime);
        }
    }
}
