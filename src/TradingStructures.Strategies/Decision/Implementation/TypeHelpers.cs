using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.Common.Structure.Results;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    internal static class TypeHelpers
    {
        internal static Result<Estimator.Type> ConvertFrom(DecisionSystem system) 
            => system switch
            {
                DecisionSystem.ArbitraryStatsLeastSquares => Estimator.Type.LeastSquares,
                DecisionSystem.FiveDayStatsLeastSquares => Estimator.Type.LeastSquares,
                DecisionSystem.FiveDayStatsLasso => Estimator.Type.LassoRegression,
                DecisionSystem.FiveDayStatsRidge => Estimator.Type.RidgeRegression,
                _ => new ErrorResult<Estimator.Type>("Argument of of supported type.")
            };
    }
}
