using System;

using Common.Structure.Reporting;

using FinancialStructures.DataStructures;

using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;

namespace TradingSystem.Trading
{
    /// <summary>
    /// Mechanism to enact the buying and selling of stocks.
    /// e.g. one could have a simulation system, or one could use this to interact with a broker.
    /// </summary>
    public interface ITradeMechanism
    {
        /// <summary>
        /// The settings for the trade submitter.
        /// </summary>
        TradeMechanismSettings Settings { get; }

        /// <summary>
        /// Enact a trade which was submitted at a given time.
        /// </summary>
        SecurityTrade Trade(
            DateTime time,
            Trade trade,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            decimal availableFunds,
            IReportLogger reportLogger);
    }
}
