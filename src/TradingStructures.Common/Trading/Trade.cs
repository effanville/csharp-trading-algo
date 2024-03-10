using System;

using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.TradingStructures.Common.Trading
{
    /// <summary>
    /// A trade of an instrument.
    /// </summary>
    public sealed class Trade : IEquatable<Trade>
    {
        /// <summary>
        /// The name data of the stock.
        /// </summary>
        public NameData StockName { get; }

        /// <summary>
        /// The decision.
        /// </summary>
        public TradeType BuySell { get; }

        public decimal NumberShares { get; }

        public decimal LimitPrice { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public Trade(NameData stock, TradeType buySell)
        {
            StockName = stock;
            BuySell = buySell;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public Trade(NameData stock, TradeType buySell, decimal numShares, decimal? limitPrice)
        {
            StockName = stock;
            BuySell = buySell;
            NumberShares = numShares;
            if (limitPrice.HasValue)
            {
                LimitPrice = limitPrice.Value;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{BuySell}-{StockName}-{NumberShares}";

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is Trade trade)
            {
                return Equals(trade);
            }

            return false;
        }

        public bool Equals(Trade other)
        {
            if (other == null)
            {
                return false;
            }

            return StockName.IsEqualTo(other.StockName)
                   && BuySell == other.BuySell
                   && NumberShares == other.NumberShares
                   && LimitPrice == other.LimitPrice;
        }

        public override int GetHashCode() => HashCode.Combine(StockName, BuySell, NumberShares, LimitPrice);

    }
}