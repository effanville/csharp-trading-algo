using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.Common.Structure.Results;

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
                    return new ErrorResult<Estimator.Type>("Argument of of supported type.");
            }
        }
    }
}
