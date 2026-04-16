namespace Effanville.TradingStructures.Common.Diagnostics
{
    public interface ITimerFactory
    {
        Timer Create(string name);
    }
}