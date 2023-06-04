using FinancialStructures.Database;

using TradingSystem.Trading;

namespace TradingSystem.MarketEvolvers
{
    /// <summary>
    /// The result of a market evolver.
    /// </summary>
    public sealed class EvolverResult
    {
        /// <summary>
        /// The ending <see cref="IPortfolio"/> from the evolution.
        /// </summary>
        public IPortfolio Portfolio
        {
            get;
        }

        /// <summary>
        /// The history of all decisions made during the evolution.
        /// </summary>
        public TradeHistory Decisions
        {
            get;
        }

        /// <summary>
        /// The history of all the trades enacted in the evolution.
        /// </summary>
        public TradeHistory Trades
        {
            get;
        }

        public EvolverResult()
        {
            Portfolio = null;
            Decisions = null;
            Trades = null;
        }

        public EvolverResult(IPortfolio portfolio, TradeHistory decisions, TradeHistory trades)
        {
            Portfolio = portfolio;
            Decisions = decisions;
            Trades = trades;
        }

        public static EvolverResult NoResult() => new EvolverResult();
    }
}
