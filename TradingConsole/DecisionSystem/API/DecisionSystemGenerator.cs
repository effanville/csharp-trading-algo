using StructureCommon.Reporting;

namespace TradingConsole.DecisionSystem
{
    internal static class DecisionSystemGenerator
    {
        internal static IDecisionSystem Generate(DecisionSystemType decisionSystemType, IReportLogger reportLogger)
        {
            switch (decisionSystemType)
            {
                case DecisionSystemType.BuyAll:
                    return new BuyAllDecisionSystem(reportLogger);
                case DecisionSystemType.ArbitraryStatsLeastSquares:
                    return new ArbitraryStatsLSDecisionSystem(reportLogger);
                case DecisionSystemType.FiveDayStatsLeastSquares:
                default:
                    return new FiveDayStatsLSDecisionSystem(reportLogger);
            }
        }
    }
}
