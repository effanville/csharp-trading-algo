using Common.Structure.Reporting;
using TradingConsole.BuySellSystem.Implementation;

namespace TradingConsole.BuySellSystem
{
    internal static class TradeMechanismFactory
    {
        internal static ITradeMechanism Create(TradeMechanismType buySellType, IReportLogger reportLogger)
        {
            switch (buySellType)
            {
                case TradeMechanismType.IB:
                    return new IBClientTradingSystem(reportLogger);
                case TradeMechanismType.SellAllThenBuy:
                default:
                    return new SimulationBuySellSystem(reportLogger);
            }
        }
    }
}
