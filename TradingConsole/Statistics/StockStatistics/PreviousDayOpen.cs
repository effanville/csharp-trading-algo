using FinancialStructures.StockStructures;
using System;
using System.Linq;

namespace TradingConsole.Statistics
{
    public class PreviousDayOpen : IStockStatistic
    {
        public StatisticType TypeOfStatistic => StatisticType.PrevDayOpen;

        public int BurnInTime => 1;

        public DataStream DataType => DataStream.Open;

        public double Calculate(DateTime date, Stock stock)
        {
            return stock.Values(date, 1, 0, DataType).First();
        }
    }
}
