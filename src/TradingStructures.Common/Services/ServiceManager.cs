using System;
using System.Collections.Generic;

namespace Effanville.TradingStructures.Common.Services;

public sealed class ServiceManager : IService
{
    private readonly Dictionary<string, IService> _registeredServices = new Dictionary<string, IService>();

    public string Name => nameof(ServiceManager);

    public void Initialize(EvolverSettings settings)
    {
        foreach (IService service in _registeredServices.Values)
        {
            service.Initialize(settings);
        }
    }

    public bool RegisterService(string name, IService service) => _registeredServices.TryAdd(name, service);

    public T GetService<T>(string name) where T : class => _registeredServices[name] as T;

    public void Restart() => throw new NotImplementedException();

    public void Shutdown()
    {
        foreach (IService service in _registeredServices.Values)
        {
            service.Shutdown();
        }
    }
}
