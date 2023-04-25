namespace TradingSystem.Simulator.Trading
{
    public sealed class TradeStatus
    {
        public int NumberBuys
        {
            get;
            set;
        }

        public int NumberSells
        {
            get;
            set;
        }

        public TradeStatus(int numberBuys, int numberSells)
        {
            NumberBuys = numberBuys;
            NumberSells = numberSells;
        }

        public override string ToString()
        {
            return $"Buys: {NumberBuys}. Sells: {NumberSells}.";
        }
    }
}
