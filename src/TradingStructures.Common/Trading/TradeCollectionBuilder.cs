using System;

using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.TradingStructures.Common.Trading
{
    public sealed class TradeCollectionBuilder
    {
        private TradeCollection _instance = new();

        public TradeCollectionBuilder Reset(DateTime start, DateTime end)
        {
            _instance = new TradeCollection(start, end);
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