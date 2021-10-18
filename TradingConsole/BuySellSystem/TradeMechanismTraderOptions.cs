using FinancialStructures.NamingStructures;

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
            double fractionInvest = 1,
            double tradeCost = 6,
            TwoName bankAccData = null)
        {
            FractionInvest = fractionInvest;
            TradeCost = tradeCost;
            BankAccData = bankAccData ?? new TwoName("Cash", "Portfolio");
        }
    }
}
