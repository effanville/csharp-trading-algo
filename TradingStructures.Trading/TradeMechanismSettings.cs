namespace Effanville.TradingStructures.Trading
{
    /// <summary>
    /// Contains options for
    /// </summary>
    public sealed class TradeMechanismSettings
    {
        /// <summary>
        /// The fixed cost associated with each trade.
        /// </summary>
        public decimal TradeCost
        {
            get;
            set;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public TradeMechanismSettings(decimal tradeCost)
        {
            TradeCost = tradeCost;
        }

        public static TradeMechanismSettings Default() => new TradeMechanismSettings(6);
    }
}
