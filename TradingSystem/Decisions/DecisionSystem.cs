namespace TradingSystem.Decisions
{
    /// <summary>
    /// The type of the decision system to be used.
    /// </summary>
    public enum DecisionSystem
    {
        FiveDayStatsLeastSquares,
        FiveDayStatsLasso,
        FiveDayStatsRidge,
        ArbitraryStatsLeastSquares,
        ArbitraryStatsLasso,
        ArbitraryStatsRidge,

        /// <summary>
        /// A system that buys everything.
        /// </summary>
        BuyAll
    }
}
