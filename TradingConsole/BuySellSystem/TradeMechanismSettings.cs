using System;
using FinancialStructures.NamingStructures;

namespace TradingConsole.BuySellSystem
{
    /// <summary>
    /// Settings for the BuySell system.
    /// These are inherent settings for how the system works.
    /// </summary>
    public sealed class TradeMechanismSettings
    {
        /// <summary>
        /// Contains a random number generator for required points.
        /// </summary>
        public Random RandomNumbers
        {
            get;
        } = new Random(12345);

        /// <summary>
        /// The probability that a stock will have gone up from the opening price.
        /// </summary>
        public double UpTickProbability
        {
            get;
            set;
        }

        /// <summary>
        /// The relative size that a stock will have increased from the opening price.
        /// </summary>
        public double UpTickSize
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
        public TradeMechanismSettings(
            double upTickProbability = 0.5,
            double upTickSize = 0.01,
            TwoName bankAccData = null)
        {
            UpTickProbability = upTickProbability;
            UpTickSize = upTickSize;
            BankAccData = bankAccData ?? new TwoName("Cash", "Portfolio");
        }
    }
}
