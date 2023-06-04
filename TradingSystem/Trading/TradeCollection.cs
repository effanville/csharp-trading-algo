using System;
using System.Collections.Generic;
using System.Linq;

using FinancialStructures.DataStructures;
using FinancialStructures.NamingStructures;

namespace TradingSystem.Trading
{
    public sealed class TradeCollection : IEquatable<TradeCollection>
    {
        public DateTime Start
        {
            get; set;
        }

        public DateTime End
        {
            get; set;
        }

        public List<Trade> Trades
        {
            get; set;
        }

        public int NumberBuys => GetBuyDecisions().Count;


        public int NumberSells => GetSellDecisions().Count;

        internal TradeCollection()
        {
            Trades = new List<Trade>();
        }

        public TradeCollection(DateTime start, DateTime end)
            : this()
        {
            Start = start;
            End = end;
        }

        public TradeCollection(DateTime start, DateTime end, List<Trade> trades)
        {
            Start = start;
            End = end;
            Trades = trades;
        }

        public void Add(NameData stock, TradeType buySell, decimal numberShares = 0m)
            => Trades.Add(new Trade(stock, buySell, numberShares));

        public List<Trade> GetBuyDecisions() => GetDecisions(TradeType.Buy);

        public List<Trade> GetSellDecisions() => GetDecisions(TradeType.Sell);

        public override string ToString()
            => $"Buys: {string.Join(",", GetBuyDecisions().Select(dec => dec.StockName))}. Sells:  {string.Join(",", GetSellDecisions().Select(dec => dec.StockName))}";

        private List<Trade> GetDecisions(TradeType buySell)
        {
            List<Trade> output = new List<Trade>();
            foreach (Trade dec in Trades)
            {
                if (dec.BuySell == buySell)
                {
                    output.Add(dec);
                }
            }
            return output;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is TradeCollection collection)
            {
                return Equals(collection);
            }

            return false;
        }
        public bool Equals(TradeCollection collection)
        {
            bool startEqual = Start == collection.Start;
            bool endEqual = End == collection.End;
            bool tradesEqual = Trades.SequenceEqual(collection.Trades);
            return startEqual
                    && endEqual
                    && tradesEqual
                    && NumberBuys == collection.NumberBuys
                    && NumberSells == collection.NumberSells;
        }

        public override int GetHashCode() => HashCode.Combine(Start, End, Trades, NumberBuys, NumberSells);
    }
}