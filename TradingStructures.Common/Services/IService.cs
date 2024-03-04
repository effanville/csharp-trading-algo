namespace Effanville.TradingStructures.Common.Services;

public interface IService
{
    string Name { get; }

    void Initialize(EvolverSettings settings);

    void Restart();
    void Shutdown();
}
