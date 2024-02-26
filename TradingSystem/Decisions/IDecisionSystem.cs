using System;

using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Trading;

using TradingSystem.MarketEvolvers;
using TradingSystem.Trading;

namespace TradingSystem.Decisions
{
    /// <summary>
    /// Interface for the system by which one decides at what point to buy and sell stocks.
    /// </summary>
    public interface IDecisionSystem
    {
        /// <summary>
        /// Sets up various parameters required in the decision system.
        /// e.g. any parameters from an estimation are set at this point.
        /// This may alter the simulation parameters (e.g. the start time of the simulation).
        /// </summary>
        void Calibrate(TimeIncrementEvolverSettings settings, IReportLogger logger);

        /// <summary>
        /// The process by which a decision on each stock in the exchange is made.
        /// </summary>
        TradeCollection Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger);
    }
}
