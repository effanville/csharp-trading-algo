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
            set { value.Sort(); fValuations = value; }
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
        public double GetValue(DateTime date)
        {
            return 1.0;
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
