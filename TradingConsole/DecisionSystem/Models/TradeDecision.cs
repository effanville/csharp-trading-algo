namespace TradingConsole.DecisionSystem.Models
{
    /// <summary>
    /// A decision type for a Trade.
    /// </summary>
    public enum TradeDecision
    {
        /// <summary>
        /// Dont know the decision.
        /// </summary>
        Unknown,

        /// <summary>
        /// The decision is to buy.
        /// </summary>
        Buy,

        /// <summary>
        /// The decision is to sell.
        /// </summary>
        Sell,

        /// <summary>
        /// The decision is to hold.
        /// </summary>
        Hold
    }
}
