using System;

using Common.Structure.Reporting;

using FinancialStructures.StockStructures;

using TradingSystem.DecideThenTradeSystem;
using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;
using TradingSystem.Trading;

namespace TradingSystem.Decisions
{
    /// <summary>
    /// A <see cref="ITradeEnactor"/> that acts simply by first deciding on
    /// what stocks to buy/sell/hold and then passing through these decisions
    /// trying to buy and sell.<para/>
    /// The buying and selling part is dealt with via an <see cref="IDecisionSystem"/>
    /// and the trading part is dealt with via a <see cref="ITradeMechanism"/>.
    /// </summary>
    public sealed class DecideThenTradeEnactor : ITradeEnactor
    {
        private readonly IDecisionSystem fDecisionSystem;
        private readonly ITradeMechanism fTradeMechanism;
        private readonly TradeMechanismTraderOptions fTraderOptions;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public DecideThenTradeEnactor(IDecisionSystem decisionSystem, ITradeMechanism tradeMechanism, TradeMechanismTraderOptions traderOptions)
        {
            fDecisionSystem = decisionSystem;
            fTradeMechanism = tradeMechanism;
            fTraderOptions = traderOptions;
        }

        /// <inheritdoc/>
        public TradeEnactorResult EnactTrades(
            DateTime time,
            IStockExchange stockExchange,
            IPortfolioManager portfolioManager,
            IPriceService priceService,
            IReportLogger reportLogger)
        {
            // Decide which stocks to buy, sell or do nothing with.
            TradeCollection status = fDecisionSystem.Decide(time, stockExchange, logger: null);

            // Exact the buy/Sell decisions.
            TradeCollection trades = fTradeMechanism.EnactAllTrades(
                time,
                status,
                priceService,
                portfolioManager,
                fTraderOptions,
                reportLogger);
            return new TradeEnactorResult(trades, status);
        }
    }
}
