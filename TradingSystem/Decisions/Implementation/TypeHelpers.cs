using Common.Structure.MathLibrary;
using Common.Structure.MathLibrary.ParameterEstimation;

namespace TradingSystem.Decisions.Implementation
{
    internal static class TypeHelpers
    {
        internal static Result<Estimator.Type> ConvertFrom(DecisionSystem system)
        {
            switch (system)
            {
                case DecisionSystem.FiveDayStatsLeastSquares:
                    return Estimator.Type.LeastSquares;
                case DecisionSystem.FiveDayStatsLasso:
                    return Estimator.Type.LassoRegression;
                case DecisionSystem.FiveDayStatsRidge:
                    return Estimator.Type.RidgeRegression;
                default:
                    return Result.ErrorResult<Estimator.Type>("Argument of of supported type.");
            }
        }
    }
}
