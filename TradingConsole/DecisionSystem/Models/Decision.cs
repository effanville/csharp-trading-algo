using FinancialStructures.NamingStructures;

namespace TradingConsole.DecisionSystem.Models
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
    }
}
