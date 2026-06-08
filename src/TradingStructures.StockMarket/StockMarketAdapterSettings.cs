namespace Effanville.TradingStructures.StockMarket
{
    /// <summary>
    /// Contains options for
    /// </summary>
    public sealed class StockMarketAdapterSettings
    {
        /// <summary>
        /// The fixed cost associated with each trade.
        /// </summary>
        public decimal TradeCost { get; }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public StockMarketAdapterSettings(decimal tradeCost)
        {
            TradeCost = tradeCost;
        }

        public static StockMarketAdapterSettings Default() => new StockMarketAdapterSettings(6);
    }
}
