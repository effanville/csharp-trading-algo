using System;

using FinancialStructures.NamingStructures;

namespace TradingConsole.TradingSystem
{
    /// <summary>
    /// Settings for constructing the start portfolio.
    /// </summary>
    public sealed class PortfolioStartSettings
    {
        /// <summary>
        /// The filepath for the start portfolio.
        /// </summary>
        public string PortfolioFilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// The start time of the simulation. This is the latest of the
        /// user specified time and the suitable start time from the Exchange data.
        /// </summary>
        public DateTime StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// The starting cash.
        /// </summary>
        public double StartingCash
        {
            get;
            private set;
        }

        /// <summary>
        /// The default bank account name to use.
        /// </summary>
        public TwoName DefaultBankAccName
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public PortfolioStartSettings(string portfolioFilePath, DateTime startDate, double startingCash)
        {
            PortfolioFilePath = portfolioFilePath;
            StartTime = startDate;
            StartingCash = startingCash;
            DefaultBankAccName = new TwoName("Cash", "Portfolio");
        }
    }
}
