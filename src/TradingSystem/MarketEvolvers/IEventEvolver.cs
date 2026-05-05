using System.Collections.Generic;

using Effanville.TradingStructures.Strategies;

namespace Effanville.TradingSystem.MarketEvolvers;

public interface IEventEvolver
{
    bool IsActive { get; }
    IReadOnlyDictionary<IStrategy, StrategyHistory?>? Result { get; }

    void Initialise();
    void Start();
    void Shutdown();
}