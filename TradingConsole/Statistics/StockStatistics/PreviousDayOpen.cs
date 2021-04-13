using System;
using System.Linq;
using FinancialStructures.StockStructures;

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

        public StockDataStream DataType
        {
            get
            {
                return StockDataStream.Open;
            }
        }

        public double Calculate(DateTime date, IStock stock)
        {
            return stock.Values(date, 1, 0, DataType).First();
        }
    }
}
