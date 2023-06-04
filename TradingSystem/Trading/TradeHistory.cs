using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public string ConvertToTable()
        {
            StringBuilder sb = new StringBuilder("|StartDate|EndDate|StockName|TradeType|NumberShares|\r\n|-|-|-|-|-|\r\n");
            foreach (var entry in DailyTrades)
            {
                foreach (var trade in entry.Value.Trades)
                {
                    _ = sb.Append('|')
                        .Append(entry.Key.ToString("yyyy-MM-ddThh:mm:ss"))
                        .Append('|')
                        .Append(entry.Key.ToString("yyyy-MM-ddThh:mm:ss"))
                        .Append('|')
                        .Append(trade.StockName.Company).Append('-').Append(trade.StockName.Name)
                        .Append('|')
                        .Append(trade.BuySell)
                        .Append('|')
                        .Append(trade.NumberShares)
                        .Append('|')
                        .AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}