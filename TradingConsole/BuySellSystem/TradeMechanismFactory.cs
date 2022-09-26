using TradingConsole.BuySellSystem.Implementation;

using TradingSystem.DecideThenTradeSystem;

namespace TradingConsole.BuySellSystem
{
    internal static class TradeMechanismFactory
    {
        internal static ITradeMechanism Create(TradeMechanismType buySellType)
        {
            switch (buySellType)
            {
                case TradeMechanismType.SellAllThenBuy:
                default:
                    return new SimulationBuySellSystem();
            }
        }
    }
}
