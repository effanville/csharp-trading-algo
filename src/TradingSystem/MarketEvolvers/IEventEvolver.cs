using Effanville.TradingStructures.Strategies;

namespace Effanville.TradingSystem.MarketEvolvers;

public interface IEventEvolver
{
    bool IsActive { get; }
    StrategyHistory Result { get; }

    void Initialise();
    void Start();
    void Shutdown();
}