using System;

namespace Effanville.TradingStructures.Common.DependencyInjection;

public sealed record CommonServiceSettings(bool IsSimulation, bool IsEventBased, DateTime StartTime);
