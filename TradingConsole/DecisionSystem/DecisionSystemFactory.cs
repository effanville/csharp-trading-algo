using Common.Structure.Reporting;

using TradingConsole.DecisionSystem.Implementation;

using TradingSystem.DecideThenTradeSystem;
using TradingSystem.Simulator;

namespace TradingConsole.DecisionSystem
{
    /// <summary>
    /// Factory for creating a decision system.
    /// </summary>
    public static partial class DecisionSystemFactory
    {
        internal static IDecisionSystem Create(Settings settings)
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

        internal static IDecisionSystem CreateAndCalibrate(Settings settings, StockMarketEvolver.Settings simulatorSettings, IReportLogger logger)
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
