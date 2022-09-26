using FinancialStructures.Database;

using TradingSystem.Simulator.Trading;
using TradingSystem.Simulator.Trading.Decisions;

namespace TradingSystem.Simulator
{
    public static partial class StockMarketEvolver
    {
        /// <summary>
        /// The result of a <see cref="StockMarketEvolver"/>
        /// </summary>
        public sealed class Result
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
            public DecisionHistory Decisions
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

            public Result()
            {
                Portfolio = null;
                Decisions = null;
                Trades = null;
            }

            public Result(IPortfolio portfolio, DecisionHistory decisions, TradeHistory trades)
            {
                Portfolio = portfolio;
                Decisions = decisions;
                Trades = trades;
            }

            public static Result NoResult()
            {
                return new Result();
            }
        }
    }
}
