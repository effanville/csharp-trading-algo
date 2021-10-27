using FinancialStructures.NamingStructures;

namespace TradingSystem.Trading.System
{
    /// <summary>
    /// Contains options for
    /// </summary>
    public sealed class TradeMechanismTraderOptions
    {
        /// <summary>
        /// The fraction of available cash to invest in any one decision.
        /// </summary>
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

        /// <summary>
        /// The default bank account name to use.
        /// </summary>
        public TwoName BankAccData
        {
            get;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public TradeMechanismTraderOptions(
            double fractionInvest = 0.25,
            double tradeCost = 6,
            TwoName bankAccData = null)
        {
            FractionInvest = fractionInvest;
            TradeCost = tradeCost;
            BankAccData = bankAccData ?? new TwoName("Cash", "Portfolio");
        }
    }
}
