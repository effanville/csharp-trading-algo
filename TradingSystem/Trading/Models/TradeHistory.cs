using System;
using System.Collections.Generic;

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
    }
}
