﻿using System;

using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Time;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Common.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection serviceCollection)
        => AddCommonServices(serviceCollection, DateTime.MinValue);

    public static IServiceCollection AddCommonServices(
        this IServiceCollection serviceCollection, DateTime startTime)
    {
        return AddCommonServices(serviceCollection,
            new CommonServiceSettings(true, true, startTime));
    }

    public static IServiceCollection AddCommonServices(
        this IServiceCollection serviceCollection, CommonServiceSettings settings)
    {
        if (!settings.IsSimulation)
        {
            serviceCollection.AddSingleton<IClock, RealTimeClock>();
        }
        else
        {
            if (settings.IsEventBased)
            {
                serviceCollection.AddSingleton<IClock, SimulationEventBasedClock>(a => new SimulationEventBasedClock(settings.StartTime));
            }
            else
            {
                serviceCollection.AddSingleton<IClock, SimulationClock>(a => new SimulationClock(settings.StartTime));
            }
        }

        return serviceCollection.AddSingleton<IScheduler, Scheduler>();
    }
}