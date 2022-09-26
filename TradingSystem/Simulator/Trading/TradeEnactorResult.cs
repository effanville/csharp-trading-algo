using TradingSystem.Simulator.Trading.Decisions;

namespace TradingSystem.Simulator.Trading
{
    public sealed class TradeEnactorResult
    {
        public TradeStatus Trades
        {
            get;
        }

        public DecisionStatus Decisions
        {
            get;
        }

        public TradeEnactorResult(TradeStatus trades, DecisionStatus decisions)
        {
            Trades = trades;
            Decisions = decisions;
        }
    }
}
