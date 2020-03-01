using FinancialStructures.GUIFinanceStructures;
using System;
using System.Collections.Generic;

namespace TradingConsole.StockStructures
{
    public class Stock
    {
        public NameData Name;

        private List<StockDayPrices> fValuations;

        /// <summary>
        /// Values associated to this stock in order earliest -> latest.
        /// </summary>
        public List<StockDayPrices> Valuations
        {
            get { return fValuations; }
            set { fValuations = value; fValuations.Sort(); }
        }

        internal int LastAccessedValuationIndex = 0;

        public Stock()
        {
        }

        public Stock(string company, string name, string url)
        {
            Name = new NameData(name.Trim(), company.Trim(), "", url.Trim());
            Valuations = new List<StockDayPrices>();
        }

        public void AddValue(DateTime time, double open, double high, double low, double close, double volume)
        {
            Valuations.Add(new StockDayPrices(time, open, high, low, close, volume));
        }

        public void Sort()
        {
            Valuations.Sort();
        }

        /// <summary>
        /// Calculates the value of the stock at the time specified.
        /// </summary>
        public double Value(DateTime date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the value on the date specified, where the last query was at index given.
        /// A quick method for getting new values.
        /// </summary>
        public double PreviousValue(DateTime date, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the value on the date specified, where the last query was at index given.
        /// A quick method for getting new values.
        /// </summary>
        public double NextValue(DateTime date, int lastIndex)
        {
            throw new NotImplementedException();
        }

        public DateTime EarliestTime()
        {
            return Valuations[0].Time;
        }

        public DateTime LastTime()
        {
            return Valuations[Valuations.Count - 1].Time;
        }
    }
}
