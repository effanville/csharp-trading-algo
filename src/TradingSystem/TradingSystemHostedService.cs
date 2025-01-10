using System.Threading;
using System.Threading.Tasks;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingSystem.MarketEvolvers;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Timer = Effanville.TradingStructures.Common.Diagnostics.Timer;

namespace Effanville.TradingSystem;

public sealed class TradingSystemHostedService : IHostedService
{
    private readonly IEventEvolver _evolver;
    private readonly ILogger<TradingSystemHostedService> _logger;
    private readonly IReportLogger _reportLogger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    
    public EvolverResult? Result { get; private set; }
    
    public TradingSystemHostedService(
        IEventEvolver evolver,
        ILogger<TradingSystemHostedService> logger,
        IReportLogger reportLogger,
        IHostApplicationLifetime applicationLifetime)
    {
        _evolver = evolver;
        _logger = logger;
        _reportLogger = reportLogger;
        _applicationLifetime = applicationLifetime;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)    
    {
        _logger.Log(LogLevel.Information, "Starting Processing.");
        _applicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(RunInBackground, cancellationToken);
        });
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)     
    {
        _logger.Log(LogLevel.Information, "Completed processing. Shutting Down.");
        return Task.CompletedTask;
    }
    
    private void RunInBackground()
    {
        try
        {
            using (new Timer(_reportLogger, "Execution"))
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