using TradingSystem.Trading.Implementation;

namespace TradingSystem.Trading
{
    public static class TradeSubmitterFactory
    {
        public static ITradeSubmitter Create(TradeSubmitterType buySellType, TradeMechanismSettings settings)
        {
            switch (buySellType)
            {
                case TradeSubmitterType.SellAllThenBuy:
                default:
                    return new SimulationBuySellSystem(settings);
            }
        }
    }
}
