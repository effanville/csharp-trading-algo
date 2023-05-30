using System;

using Common.Structure.Reporting;

using TradingSystem.DecideThenTradeSystem;
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
        /// Enact a buy decision.
        /// </summary>
        bool Buy(
            DateTime time,
            Trade buy,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);

        /// <summary>
        /// Enact a sell decision
        /// </summary>
        bool Sell(
            DateTime time,
            Trade sell,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);

        /// <summary>
        /// Routine to enact all trades.
        /// </summary>
        TradeCollection EnactAllTrades(
            DateTime time,
            TradeCollection decisions,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger);
    }
}
