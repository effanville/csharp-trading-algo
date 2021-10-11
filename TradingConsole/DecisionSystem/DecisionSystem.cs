namespace TradingConsole.DecisionSystem
{
    /// <summary>
    /// The type of the decision system to be used.
    /// </summary>
    public enum DecisionSystem
    {
        FiveDayStatsLeastSquares,
        ArbitraryStatsLeastSquares,

        /// <summary>
        /// A system that buys everything.
        /// </summary>
        BuyAll
    }
}
