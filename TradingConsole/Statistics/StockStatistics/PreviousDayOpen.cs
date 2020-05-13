using FinancialStructures.StockStructures;
using System;
using System.Linq;

namespace TradingConsole.Statistics
{
    public class PreviousDayOpen : IStockStatistic
    {
        public StatisticType TypeOfStatistic
        {
            get
            {
                return StatisticType.PrevDayOpen;
            }
        }

        public int BurnInTime
        {
            get
            {
                return 1;
            }
        }

        public DataStream DataType
        {
            get
            {
                return DataStream.Open;
            }
        }

        public double Calculate(DateTime date, Stock stock)
        {
            return stock.Values(date, 1, 0, DataType).First();
        }
    }
}
