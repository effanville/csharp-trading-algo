using TradingConsole.BuySellSystem.Implementation;

using TradingSystem.Trading.System;

namespace TradingConsole.BuySellSystem
{
    internal static class TradeMechanismFactory
    {
        internal static ITradeMechanism Create(TradeMechanismType buySellType)
        {
            switch (buySellType)
            {
                case TradeMechanismType.IB:
                    return new IBClientTradingSystem();

                case TradeMechanismType.SellAllThenBuy:
                default:
                    return new SimulationBuySellSystem();
            }
        }
    }
}
