namespace TradingConsole.BuySellSystem
{
    public sealed class TradeMechanismTraderOptions
    {
        public double FractionInvest
        {
            get;
            set;
        }

        /// <summary>
        /// The fixed cost associated with each trade.
        /// </summary>
        public double TradeCost
        {
            get;
            set;
        }

        public TradeMechanismTraderOptions(double fractionInvest = 1, double tradeCost = 6)
        {
            FractionInvest = fractionInvest;
            TradeCost = tradeCost;
        }
    }
}
