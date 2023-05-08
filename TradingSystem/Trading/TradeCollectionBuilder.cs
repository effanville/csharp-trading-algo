using System;
using System.Collections.Generic;

using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;

namespace TradingSystem.Trading
{
    public sealed class TradeCollectionBuilder
    {
        private TradeCollection _instance;
        private Dictionary<DateTime, TradeCollection> _tradeCollectionDictionary;
        public TradeCollectionBuilder()
        {
            _instance = new TradeCollection();
        }

        public TradeCollectionBuilder SetDates(DateTime start, DateTime end)
        {
            _instance.Start = start;
            _instance.End = end;
            return this;
        }

        public TradeCollectionBuilder Add(NameData stock, TradeType buySell, decimal numberShares = 0m)
        {
            _instance.Add(stock, buySell, numberShares);
            return this;
        }

        public TradeCollectionBuilder Reset()
        {
            _instance = new TradeCollection();
            return this;
        }

        public TradeCollection GetSingleInstance() => _instance;
    }
}