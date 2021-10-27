using TradingConsole.DecisionSystem.Implementation;

using TradingSystem.Decisions.System;

namespace TradingConsole.DecisionSystem
{
    internal static class DecisionSystemFactory
    {
        internal static IDecisionSystem Create(DecisionSystemSetupSettings settings)
        {
            switch (settings.DecisionSystemType)
            {
                case DecisionSystem.BuyAll:
                    return new BuyAllDecisionSystem();

                case DecisionSystem.ArbitraryStatsLeastSquares:
                    return new ArbitraryStatsDecisionSystem(settings);
                case DecisionSystem.ArbitraryStatsLasso:
                    return new ArbitraryStatsDecisionSystem(settings);
                case DecisionSystem.ArbitraryStatsRidge:
                    return new ArbitraryStatsDecisionSystem(settings);
                case DecisionSystem.FiveDayStatsLasso:
                    return new FiveDayStatsDecisionSystem(settings);
                case DecisionSystem.FiveDayStatsRidge:
                    return new FiveDayStatsDecisionSystem(settings);
                case DecisionSystem.FiveDayStatsLeastSquares:
                default:
                    return new FiveDayStatsDecisionSystem(settings);
            }
        }
    }
}
