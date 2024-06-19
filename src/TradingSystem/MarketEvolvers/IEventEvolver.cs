namespace Effanville.TradingSystem.MarketEvolvers;

public interface IEventEvolver
{
    bool IsActive { get; }
    EvolverResult Result { get; }

    void Initialise();
    void Start();
    void Shutdown();
}