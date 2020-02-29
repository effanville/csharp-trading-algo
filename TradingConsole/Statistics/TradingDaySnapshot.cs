using FinancialStructures.GUIFinanceStructures;
using System;
using System.Collections.Generic;

namespace TradingConsole.Statistics
{
    public class TradingDaySnapshot
    {
        public DateTime Time;
        public List<Holding> stocksHeld;
        public double freeCash;

        public void AddHolding(NameData name, DayDataView data)
        {
            stocksHeld.Add(new Holding(name, data));
        }
    }

    public class Holding
    {
        public Holding(NameData name, DayDataView data)
        {
            stock = name;
            priceData = data;
        }

        public NameData stock;
        public DayDataView priceData;
    }
}
