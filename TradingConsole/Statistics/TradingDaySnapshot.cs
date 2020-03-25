using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;
using System;
using System.Collections.Generic;

namespace TradingConsole.Statistics
{
    public class TradingDaySnapshot
    {
        public DateTime Time;
        public List<Holding> stocksHeld = new List<Holding>();
        public double freeCash;

        public void AddHolding(NameData name, SecurityDayData data)
        {
            stocksHeld.Add(new Holding(name, data));
        }
    }

    public class Holding
    {
        public Holding(NameData name, SecurityDayData data)
        {
            stock = name;
            priceData = data;
        }

        public NameData stock;
        public SecurityDayData priceData;
    }
}
