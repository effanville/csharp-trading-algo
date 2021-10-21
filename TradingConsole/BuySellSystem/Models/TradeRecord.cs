using System;
using System.Collections.Generic;

namespace TradingConsole.BuySellSystem.Models
{
    public sealed class TradeRecord
    {
        public Dictionary<DateTime, TradeStatus> DailyTrades
        {
            get;
        }

        public TradeRecord()
        {
            DailyTrades = new Dictionary<DateTime, TradeStatus>();
        }

        public void AddForTheRecord(DateTime day, TradeStatus status)
        {
            DailyTrades.Add(day, status);
        }
    }
}
