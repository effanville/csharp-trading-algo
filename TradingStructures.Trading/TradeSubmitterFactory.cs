using Effanville.TradingStructures.Trading.Implementation;

namespace Effanville.TradingStructures.Trading
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
