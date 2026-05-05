using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingSystem.MarketEvolvers;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingSystem;

public sealed class TradingSystemHostedService : IHostedService
{
    private readonly IEventEvolver _evolver;
    private readonly ILogger<TradingSystemHostedService> _logger;
    private readonly ITimerFactory _timerFactory;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public IReadOnlyDictionary<IStrategy, StrategyHistory?>? Result { get; private set; }

    public TradingSystemHostedService(
        IEventEvolver evolver,
        ILogger<TradingSystemHostedService> logger,
        ITimerFactory timerFactory,
        IHostApplicationLifetime applicationLifetime)
    {
        _evolver = evolver;
        _logger = logger;
        _timerFactory = timerFactory;
        _applicationLifetime = applicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Processing.");
        _applicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(RunInBackground, cancellationToken);
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completed processing. Shutting Down.");
        return Task.CompletedTask;
    }

    private void RunInBackground()
    {
        try
        {
            using (_timerFactory.Create("Execution"))
            {
                _evolver.Initialise();
                _evolver.Start();
                while (_evolver.IsActive)
                {
                    _ = Task.Delay(100);
                }

                Result = _evolver.Result;
            }
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }
}