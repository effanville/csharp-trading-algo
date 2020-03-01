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

        public StockDayPrices()
        { }

        public StockDayPrices(DateTime time, double open, double high, double low, double close, double volume)
        {
            Time = time;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        public int CompareTo(object obj)
        {
            if (obj is StockDayPrices otherPrice)
            {
                return Time.CompareTo(otherPrice.Time);
            }

            return 0;
        }
    }
}
