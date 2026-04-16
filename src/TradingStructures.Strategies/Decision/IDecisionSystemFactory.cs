namespace Effanville.TradingStructures.Strategies.Decision;

public interface IDecisionSystemFactory
{
    IDecisionSystem Create(DecisionSystemFactory.Settings settings);
}
