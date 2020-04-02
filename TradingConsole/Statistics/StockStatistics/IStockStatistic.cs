using FinancialStructures.StockStructures;
using System;

namespace TradingConsole.Statistics
{
    public interface IStockStatistic
    {
        StatisticType TypeOfStatistic { get; }

        double Calculate(DateTime date, Stock stock);
    }
}
