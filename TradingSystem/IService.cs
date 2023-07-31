using TradingSystem.MarketEvolvers;

namespace TradingSystem;

public interface IService
{
    string Name { get; }

    void Initialize(EvolverSettings settings);

    void Restart();
    void Shutdown();
}
