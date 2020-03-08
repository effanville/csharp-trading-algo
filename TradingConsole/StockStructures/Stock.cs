using FinancialStructures.GUIFinanceStructures;
using System;
using System.Collections.Generic;

namespace TradingConsole.StockStructures
{
    public partial class Stock
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
    }
}
