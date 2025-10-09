using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.DependencyInjection;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Exchanges.DependencyInjection;
using Effanville.TradingStructures.OrderManagement;
using Effanville.TradingStructures.OrderManagement.DependencyInjection;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.StaticData.DependencyInjection;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Trading.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingSystem.MarketEvolvers;

/// <summary>
/// An evolver for a Stock market that is based on events being raised. Either
/// Exchange change events or price changed events (or others).
/// </summary>
internal sealed class EventEvolver : IEventEvolver
{
    private bool _isInitialised;
    private readonly IClock _clock;
    private readonly EvolverSettings _settings;
    private readonly IReportLogger _logger;
    private readonly IScheduler _scheduler;
    private readonly ServiceProvider _serviceProvider;
    private readonly IPriceService _priceService;
    private readonly IStrategy _strategy;
    private readonly IExchangeSessionService _exchange;
    private readonly IOrderManagementService _oms;

    /// <summary>
    /// Whether this evolution is still running.
    /// </summary>
    public bool IsActive { get; private set; }

    public StrategyHistory? Result { get; private set; }

    public EventEvolver(
        EvolverSettings settings,
        IStockExchange exchange,
        IStrategy strategy,
        IReportLogger logger)
    {
        _settings = settings;
        _logger = logger;
        IServiceCollection serviceCollection = new ServiceCollection();
        _ = serviceCollection
            .AddSingleton(a => logger)
            .AddCommonServices(settings.StartTime)
            .AddSingleton(a => exchange)
            .AddSingleton(a => strategy)
            .AddSingleton<IService>(x => x.GetService<IStrategy>())
            .AddStaticDataServices()
            .AddExchangeServices()

            // the following two could be replace with actual exchange and price connections
            // for live trading. The other parts should be able to stay as is.
            .AddSimulationExchange()
            .AddPriceService()
            .AddOrderManagement();

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _clock = _serviceProvider.GetService<IClock>()!;
        _scheduler = _serviceProvider.GetService<IScheduler>()!;
        _priceService = _serviceProvider.GetService<IPriceService>()!;
        _exchange = _serviceProvider.GetService<IExchangeSessionService>()!;
        _oms = _serviceProvider.GetService<IOrderManagementService>()!;
        _strategy = strategy;
        _ = strategy.RegisterServices(_serviceProvider);
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

        _strategy.SubmitTradeEvent += _oms.OnTradeRequested;
        _exchange.ExchangeStatusChanged += _strategy.OnExchangeStatusChanged;
        _priceService.PriceChanged += _strategy.OnPriceUpdate;

        _oms.TradeCompleted += _strategy.OnTradeConfirmed;
        _scheduler.ScheduleNewEvent(Shutdown, _settings.EndTime);
        _scheduler.ScheduleNewEvent(TimeUpdate, _clock.UtcNow().AddDays(1));
        _isInitialised = true;
        _logger.Log(ReportType.Information, nameof(EventEvolver), "Initialization complete");
    }

    private void TimeUpdate()
    {
        var time = _clock.UtcNow();
        _strategy?.OnTimeIncrementUpdate(null, new TimeIncrementEventArgs(time));
        _scheduler.ScheduleNewEvent(TimeUpdate, time.AddDays(1));
    }

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

        Result = _strategy?.History;
        IsActive = false;
    }
}