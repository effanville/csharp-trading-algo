using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Strategies.Decision.Implementation;

namespace Effanville.TradingStructures.Strategies.Decision
{
    /// <summary>
    /// Factory for creating a decision system.
    /// </summary>
    public static partial class DecisionSystemFactory
    {
        public static IDecisionSystem Create(Settings settings)
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

        public static IDecisionSystem CreateAndCalibrate(Settings settings, DecisionSystemSettings decisionSettings, IReportLogger logger)
        {
            var decisionSystem = Create(settings);
            decisionSystem.Calibrate(decisionSettings, logger);
            if (settings.IsBurnInRequired())
            {
                decisionSettings.DoesntRequireBurnIn();
            }

            return decisionSystem;
        }
    }
}
