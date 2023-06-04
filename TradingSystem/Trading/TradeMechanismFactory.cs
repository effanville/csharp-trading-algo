using TradingSystem.Trading.Implementation;

namespace TradingSystem.Trading
{
    public static class TradeMechanismFactory
    {
        public static ITradeMechanism Create(TradeMechanismType buySellType, TradeMechanismSettings settings)
        {
            switch (buySellType)
            {
                case TradeMechanismType.SellAllThenBuy:
                default:
                    return new SimulationBuySellSystem(settings);
            }
        }
    }
}
