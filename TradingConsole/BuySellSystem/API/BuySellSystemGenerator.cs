using Common.Structure.Reporting;

namespace TradingConsole.BuySellSystem
{
    internal static class BuySellSystemGenerator
    {
        internal static IBuySellSystem Generate(BuySellType buySellType, IReportLogger reportLogger)
        {
            switch (buySellType)
            {
                case BuySellType.IB:
                    return new IBClientTradingSystem(reportLogger);
                case BuySellType.Simulate:
                default:
                    return new SimulationBuySellSystem(reportLogger);
            }
        }
    }
}
