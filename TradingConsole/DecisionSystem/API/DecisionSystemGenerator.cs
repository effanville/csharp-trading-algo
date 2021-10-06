using Common.Structure.Reporting;

namespace TradingConsole.DecisionSystem
{
    internal static class DecisionSystemGenerator
    {
        internal static IDecisionSystem Generate(DecisionSystem decisionSystemType, IReportLogger reportLogger)
        {
            switch (decisionSystemType)
            {
                case DecisionSystem.BuyAll:
                    return new BuyAllDecisionSystem(reportLogger);
                case DecisionSystem.ArbitraryStatsLeastSquares:
                    return new ArbitraryStatsLSDecisionSystem(reportLogger);
                case DecisionSystem.FiveDayStatsLeastSquares:
                default:
                    return new FiveDayStatsLSDecisionSystem(reportLogger);
            }
        }
    }
}
