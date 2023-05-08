namespace TradingSystem.Trading
{
    public sealed class TradeEnactorResult
    {
        public TradeCollection Trades
        {
            get;
        }

        public TradeCollection Decisions
        {
            get;
        }

        public TradeEnactorResult(TradeCollection trades, TradeCollection decisions)
        {
            Trades = trades;
            Decisions = decisions;
        }
    }
}
