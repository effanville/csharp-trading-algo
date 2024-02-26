using System;

using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Pricing;

namespace TradingSystem.Trading
{
    /// <summary>
    /// Mechanism to enact the buying and selling of stocks.
    /// e.g. one could have a simulation system, or one could use this to interact with a broker.
    /// </summary>
    public interface ITradeSubmitter : IService
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
            decimal availableFunds,
            IReportLogger reportLogger);
    }
}
