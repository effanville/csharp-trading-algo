using FinancialStructures.NamingStructures;

namespace TradingSystem.Simulator.Trading.Decisions
{
    /// <summary>
    ///
    /// </summary>
    public sealed class Decision
    {
        /// <summary>
        /// The name data of the stock.
        /// </summary>
        public NameData StockName;

        /// <summary>
        /// The decision.
        /// </summary>
        public TradeDecision BuySell;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public Decision(NameData stock, TradeDecision buySell)
        {
            StockName = stock;
            BuySell = buySell;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{BuySell}-{StockName}";
        }
    }
}
