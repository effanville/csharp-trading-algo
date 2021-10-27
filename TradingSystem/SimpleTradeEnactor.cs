using System;

using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

using TradingSystem.Decisions.Models;
using TradingSystem.Decisions.System;
using TradingSystem.Simulator;
using TradingSystem.Trading.Models;
using TradingSystem.Trading.System;

namespace TradingSystem
{
    /// <summary>
    /// A <see cref="ITradeEnactor"/> that acts simply by first deciding on
    /// what stocks to buy/sell/hold and then passing through these decisions
    /// trying to buy and sell.<para/>
    /// The buying and selling part is dealt with via an <see cref="IDecisionSystem"/>
    /// and the trading part is dealt with via a <see cref="ITradeMechanism"/>.
    /// </summary>
    public sealed class SimpleTradeEnactor : ITradeEnactor
    {
        private readonly IDecisionSystem fDecisionSystem;
        private readonly ITradeMechanism fTradeMechanism;
        private readonly TradeMechanismTraderOptions fTraderOptions;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SimpleTradeEnactor(IDecisionSystem decisionSystem, ITradeMechanism tradeMechanism, TradeMechanismTraderOptions traderOptions)
        {
            fDecisionSystem = decisionSystem;
            fTradeMechanism = tradeMechanism;
            fTraderOptions = traderOptions;
        }

        /// <inheritdoc/>
        public (TradeStatus, DecisionStatus) EnactTrades(DateTime time, IStockExchange stockExchange, IPortfolio portfolio, Func<DateTime, TwoName, double> calcBuyPrice, Func<DateTime, TwoName, double> calcSellPrice, IReportLogger reportLogger)
        {
            // Decide which stocks to buy, sell or do nothing with.
            DecisionStatus status = fDecisionSystem.Decide(time, stockExchange, logger: null);

            // Exact the buy/Sell decisions.
            TradeStatus trades = fTradeMechanism.EnactAllTrades(
                time,
                status,
                (date, name) => calcBuyPrice(date, name),
                (date, name) => calcSellPrice(date, name),
                portfolio,
                fTraderOptions,
                reportLogger);
            return (trades, status);
        }
    }
}
