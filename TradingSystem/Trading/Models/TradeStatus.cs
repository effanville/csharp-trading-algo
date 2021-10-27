namespace TradingSystem.Trading.Models
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
    }
}
