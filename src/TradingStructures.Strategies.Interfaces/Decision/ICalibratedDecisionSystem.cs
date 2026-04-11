using Effanville.Common.Structure.MathLibrary.ParameterEstimation;

namespace Effanville.TradingStructures.Strategies.Decision;

public interface ICalibratedDecisionSystem : IDecisionSystem
{
    Estimator.Result? Result { get; }
}
