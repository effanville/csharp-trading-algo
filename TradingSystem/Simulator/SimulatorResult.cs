using FinancialStructures.Database;

using TradingSystem.Decisions.Models;
using TradingSystem.Trading.Models;

namespace TradingSystem.Simulator
{
    public sealed class SimulatorResult
    {
        public IPortfolio Portfolio
        {
            get;
        }

        public DecisionHistory Decisions
        {
            get;
        }
        public TradeHistory Trades
        {
            get;
        }

        public SimulatorResult()
        {
            Portfolio = null;
            Decisions = null;
            Trades = null;
        }
        public SimulatorResult(IPortfolio portfolio, DecisionHistory decisions, TradeHistory trades)
        {
            Portfolio = portfolio;
            Decisions = decisions;
            Trades = trades;
        }

        public static SimulatorResult NoResult()
        {
            return new SimulatorResult();
        }
    }
}
