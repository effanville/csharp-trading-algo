using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.DependencyInjection;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Exchanges.DependencyInjection;
using Effanville.TradingStructures.MarketData;
using Effanville.TradingStructures.MarketData.DependencyInjection;
using Effanville.TradingStructures.OrderManagement;
using Effanville.TradingStructures.OrderManagement.DependencyInjection;
using Effanville.TradingStructures.StaticData.DependencyInjection;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Trading.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Effanville.TradingSystem.MarketEvolvers;

/// <summary>
/// An evolver for a Stock market that is based on events being raised. Either
/// Exchange change events or price changed events (or others).
/// </summary>
public sealed class EventEvolver : IEventEvolver
{
    bool _isInitialised;
    private readonly IClock _clock;
    readonly EvolverSettings _settings;
    private readonly ILogger<EventEvolver> _logger;
    readonly IScheduler _scheduler;
    private readonly ServiceProvider _serviceProvider;
    private readonly IPriceService _priceService;
    private readonly IExchangeSessionService _exchange;
    private readonly IReadOnlyList<IStrategy> _strategies;
    private readonly IOrderManagementService _oms;

    /// <summary>
    /// Whether this evolution is still running.
    /// </summary>
    public bool IsActive
    {
        get; private set;
    }

    public IReadOnlyDictionary<IStrategy, StrategyHistory?>? Result { get; private set; }

    public EventEvolver(
        ILogger<EventEvolver> logger,
        EvolverSettings settings,
        IStockExchange exchange,
        IEnumerable<IStrategy> strategies,
        IReportLogger reportLogger)
    {
        _logger = logger;
        _settings = settings;
        IServiceCollection serviceCollection = new ServiceCollection();
        _ = serviceCollection
            .AddSingleton(a => reportLogger)
            .AddCommonServices(settings.StartTime)
            .AddSingleton(a => exchange)
            .AddStaticDataServices()
            .AddExchangeServices()

            // the following two could be replace with actual exchange and price connections
            // for live trading. The other parts should be able to stay as is.
            .AddSimulationExchange()
            .AddPriceService()
            .AddOrderManagement();

        foreach (IStrategy strategy in strategies)
        {
            _ = serviceCollection
                .AddSingleton(strategy)
                .AddSingleton<IService>(strategy);
        }

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _clock = _serviceProvider.GetService<IClock>()!;
        _scheduler = _serviceProvider.GetService<IScheduler>()!;
        _exchange = _serviceProvider.GetService<IExchangeSessionService>()!;
        _priceService = _serviceProvider.GetService<IPriceService>()!;
        _oms = _serviceProvider.GetRequiredService<IOrderManagementService>();

        _strategies = _serviceProvider.GetServices<IStrategy>().ToList();
        foreach (IStrategy strategy in _strategies)
        {
            _ = strategy.RegisterServices(_serviceProvider);
        }
    }

    /// <summary>
    /// Setup all parameters and event listening for the various different parts.
    /// </summary>
    public void Initialise()
    {
        var services = _serviceProvider.GetServices<IService>();
        foreach (IService service in services)
        {
            service.Initialize(_settings);
        }

        foreach (IStrategy strategy in _strategies)
        {
            strategy.SubmitTradeEvent += _oms.OnTradeRequested;
            _exchange.ExchangeStatusChanged += strategy.OnExchangeStatusChanged;
            _priceService.PriceChanged += strategy.OnPriceUpdate;

            _oms.TradeCompleted += strategy.OnTradeConfirmed;
        }

        ScheduleShutdown();
        _scheduler.ScheduleNewEvent(TimeUpdate, _clock.UtcNow().AddDays(1));
        _isInitialised = true;
        _logger.LogInformation("Initialization complete");
    }

    private void TimeUpdate()
    {
        var time = _clock.UtcNow();
        foreach (IStrategy strategy in _strategies)
        {
            strategy.OnTimeIncrementUpdate(null, new TimeIncrementEventArgs(time));
        }

        _scheduler.ScheduleNewEvent(TimeUpdate, time.AddDays(1));
    }

    private void ScheduleShutdown() => _scheduler.ScheduleNewEvent(Shutdown, _settings.EndTime);

    /// <summary>
    /// Start the clock running and the trading.
    /// </summary>
    public void Start()
    {
        IsActive = true;
        if (!_isInitialised)
        {
            Initialise();
        }

        _scheduler.Start();
        _clock.Start();
    }

    /// <summary>
    /// End everything from running and record the results.
    /// </summary>
    public void Shutdown()
    {
        _clock.Stop();
        _scheduler.Stop();
        foreach (IService service in _serviceProvider.GetServices<IService>())
        {
            service.Shutdown();
        }
        var results = new Dictionary<IStrategy, StrategyHistory?>(); ;
        foreach (IStrategy strategy in _strategies)
        {
            results[strategy] = strategy?.History;
        }
        Result = results;

        IsActive = false;
    }
}