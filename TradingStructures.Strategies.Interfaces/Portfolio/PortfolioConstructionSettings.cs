namespace Effanville.TradingStructures.Strategies.Portfolio
{
    /// <summary>
    /// Contains options for
    /// </summary>
    public sealed class PortfolioConstructionSettings
    {
        /// <summary>
        /// The fraction of available cash to invest in any one decision.
        /// </summary>
        public decimal FractionInvest
        {
            get;
            set;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public PortfolioConstructionSettings(decimal fractionInvest)
        {
            FractionInvest = fractionInvest;
        }

        public static PortfolioConstructionSettings Default()
            => new PortfolioConstructionSettings(0.25m);
    }
}