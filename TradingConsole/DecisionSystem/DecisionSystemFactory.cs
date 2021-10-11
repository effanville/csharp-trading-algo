using Common.Structure.Reporting;
using TradingConsole.DecisionSystem.Implementation;

namespace TradingConsole.DecisionSystem
{
    internal static class DecisionSystemFactory
    {
        internal static IDecisionSystem Create(DecisionSystem decisionSystemType, IReportLogger reportLogger)
        {
            switch (decisionSystemType)
            {
                case DecisionSystem.BuyAll:
                    return new BuyAllDecisionSystem();
                case DecisionSystem.ArbitraryStatsLeastSquares:
                    return new ArbitraryStatsLSDecisionSystem();
                case DecisionSystem.FiveDayStatsLeastSquares:
                default:
                    return new FiveDayStatsLSDecisionSystem(reportLogger);
            }
        }
    }
}
