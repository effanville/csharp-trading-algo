using FinancialStructures.NamingStructures;

namespace TradingSystem.DecideThenTradeSystem
{
    /// <summary>
    /// Contains options for
    /// </summary>
    public sealed class TradeMechanismTraderOptions
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
        /// The fixed cost associated with each trade.
        /// </summary>
        public decimal TradeCost
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
            decimal fractionInvest = 0.25m,
            decimal tradeCost = 6,
            TwoName bankAccData = null)
        {
            FractionInvest = fractionInvest;
            TradeCost = tradeCost;
            BankAccData = bankAccData ?? new TwoName("Cash", "Portfolio");
        }
    }
}
