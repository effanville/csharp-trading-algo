using System;

namespace Effanville.TradingStructures.Common.DependencyInjection;

public sealed class CommonServiceSettings
{
    public bool IsSimulation { get; }

    public bool IsEventBased { get; }
    public DateTime StartTime { get; }

    public CommonServiceSettings(bool isSimulation, bool isEventBased, DateTime startTime)
    {
        IsSimulation = isSimulation;
        IsEventBased = isEventBased;
        StartTime = startTime;
    }
}
