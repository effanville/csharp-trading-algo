using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.DependencyInjection;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Exchanges.DependencyInjection;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Pricing.DependencyInjection;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Trading;
using Effanville.TradingStructures.Trading.DependencyInjection;
using Effanville.TradingSystem.Trading;

using Microsoft.Extensions.DependencyInjection;

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
    readonly IReportLogger _logger;
    readonly IScheduler _scheduler;
    private readonly ServiceProvider _serviceProvider;
    private readonly IPriceService _priceService;
    private readonly IExchangeSessionService _exchange;
    private readonly IMarketExchange _simulationExchange;
    private readonly IOrderListener _orderListener;
    private readonly IStrategy _strategy;

    /// <summary>
    /// Whether this evolution is still running.
    /// </summary>
    public bool IsActive
    {
        get; private set;
    }

    public EvolverResult Result
    {
        get;
    } = new();

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
            .AddSingleton<IService>(x => x.GetService<IStrategy>()!)
            .AddExchangeServices()

            // the following two could be replace with actual exchange and price connections
            // for live trading. The other parts should be able to stay as is.
            .AddSimulationExchange()
            .AddPriceService()
            .AddSingleton(x => strategy.PortfolioManager)
            .AddSingleton(Result)
            .AddOrderManagement();

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _clock = _serviceProvider.GetService<IClock>()!;
        _scheduler = _serviceProvider.GetService<IScheduler>()!;
        _exchange = _serviceProvider.GetService<IExchangeSessionService>()!;
        _priceService = _serviceProvider.GetService<IPriceService>()!;
        _orderListener = _serviceProvider.GetService<IOrderListener>()!;
        _strategy = _serviceProvider.GetService<IStrategy>()!;
        strategy.RegisterServices(_serviceProvider);

        _simulationExchange = _serviceProvider.GetService<IMarketExchange>()!;
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

        _orderListener.Initialize(_settings);
        _strategy.SubmitTradeEvent += _orderListener.OnTradeRequested;
        _exchange.ExchangeStatusChanged += _strategy.OnExchangeStatusChanged;
        _priceService.PriceChanged += _strategy.OnPriceUpdate;

        _orderListener.SubmitTrade += _simulationExchange.OnTradeRequested;
        _simulationExchange.TradeCompleted += _orderListener.OnTradeConfirmed;
        ScheduleShutdown();
        _scheduler.ScheduleNewEvent(TimeUpdate, _clock.UtcNow().AddDays(1));
        _isInitialised = true;
        _logger.Log(ReportType.Information, nameof(EventEvolver), "Initialization complete");
    }

    private void TimeUpdate()
    {
        var time = _clock.UtcNow();
        _strategy.OnTimeIncrementUpdate(null, new TimeIncrementEventArgs(time));
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

        _orderListener.Shutdown();
        Result.Portfolio = _strategy.PortfolioManager.Portfolio;
        IsActive = false;
    }
}