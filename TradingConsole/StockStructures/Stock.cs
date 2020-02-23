using FinancialStructures.GUIFinanceStructures;
using System;
using System.Collections.Generic;

namespace TradingConsole.StockStructures
{
    public class Stock
    {
        public double GetValue(DateTime date)
        {
            return 1.0;
        }

        public NameData Name;

        public List<StockDailySnapshot> Valuations;
    }
}
