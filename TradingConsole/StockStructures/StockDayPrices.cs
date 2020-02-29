using System;

namespace TradingConsole.StockStructures
{
    public class StockDayPrices : IComparable
    {
        public DateTime Time;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public double Volume;

        public int CompareTo(object obj)
        {
            if (obj is DateTime date)
            {
                return Time.CompareTo(date);
            }
            return 0;
        }
    }
}
