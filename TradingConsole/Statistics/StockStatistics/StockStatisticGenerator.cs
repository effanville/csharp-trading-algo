using FinancialStructures.StockStructures;

namespace TradingConsole.Statistics
{
    public static class StockStatisticGenerator
    {
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
                case StatisticType.PrevOneOpen:
                default:
                    return new PreviousDayOpen();
            }
        }
    }
}
