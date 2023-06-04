using Common.Structure.Reporting;

using TradingSystem.Decisions.Implementation;
using TradingSystem.MarketEvolvers;

namespace TradingSystem.Decisions
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

        public static IDecisionSystem CreateAndCalibrate(Settings settings, EvolverSettings simulatorSettings, IReportLogger logger)
        {
            var decisionSystem = Create(settings);
            decisionSystem.Calibrate(simulatorSettings, logger);
            if (settings.IsBurnInRequired())
            {
                simulatorSettings.DoesntRequireBurnIn();
            }

            return decisionSystem;
        }
    }
}
