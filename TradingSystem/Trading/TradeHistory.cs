using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingSystem.Trading
{
    public sealed class TradeHistory
    {
        public Dictionary<DateTime, TradeCollection> DailyTrades { get; }

        public TradeHistory()
        {
            DailyTrades = new Dictionary<DateTime, TradeCollection>();
        }

        public void AddIfNotNull(DateTime date, TradeCollection tradeCollection)
        {
            if (tradeCollection == null)
            {
                return;
            }

            Add(date, tradeCollection);
        }

        public void Add(DateTime day, Trade trade) => Add(day, new List<Trade> { trade });

        public void Add(DateTime day, List<Trade> trades) => Add(day, new TradeCollection(day, day, trades));

        public void Add(DateTime day, TradeCollection status)
        {
            if (DailyTrades.TryGetValue(day, out TradeCollection value))
            {
                foreach (Trade trade in status.Trades)
                {
                    value.Trades.Add(trade);
                }

                return;
            }

            DailyTrades.Add(day, status);
        }

        public int TotalTrades => DailyTrades.Sum(trade => trade.Value.NumberBuys + trade.Value.NumberSells);

        public int TotalBuyTrades => DailyTrades.Sum(trade => trade.Value.NumberBuys);

        public int TotalSellTrades => DailyTrades.Sum(trade => trade.Value.NumberSells);
    }
}