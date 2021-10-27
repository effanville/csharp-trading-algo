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
                    return new ArbitraryStatsLSDecisionSystem(settings);

                case DecisionSystem.FiveDayStatsLeastSquares:
                default:
                    return new FiveDayStatsLSDecisionSystem();
            }
        }
    }
}
