using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingSystem.Trading.Models
{
    public sealed class TradeHistory
    {
        public Dictionary<DateTime, TradeStatus> DailyTrades
        {
            get;
        }

        public TradeHistory()
        {
            DailyTrades = new Dictionary<DateTime, TradeStatus>();
        }

        public void AddForTheRecord(DateTime day, TradeStatus status)
        {
            DailyTrades.Add(day, status);
        }

        public int TotalTrades()
        {
            return DailyTrades.Sum(trade => trade.Value.NumberBuys + trade.Value.NumberSells);
        }

        public int TotalBuyTrades()
        {
            return DailyTrades.Sum(trade => trade.Value.NumberBuys);
        }

        public int TotalSellTrades()
        {
            return DailyTrades.Sum(trade => trade.Value.NumberSells);
        }
    }
}
