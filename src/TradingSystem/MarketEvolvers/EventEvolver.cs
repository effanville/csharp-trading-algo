using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Trading;
using Effanville.TradingStructures.Trading.Implementation;
using Effanville.TradingSystem.Trading;

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
    private readonly IPriceService _priceService;
    private readonly IExchangeSessionService _exchange;
    private readonly SimulationExchange _simulationExchange;
    private readonly IOrderListener _orderListener;
    private IStrategy _strategy;

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
        _clock = new SimulationEventBasedClock(settings.StartTime);
        _scheduler = new Scheduler(_clock);

        _exchange = new ExchangeSessionService(_scheduler, exchange);

        _priceService = PriceServiceFactory.Create(PriceType.RandomWobble, PriceCalculationSettings.Default(), exchange, _scheduler);
        _strategy = strategy;
        strategy.RegisterClock(_clock);
        strategy.RegisterPriceService(_priceService);

        _simulationExchange = new SimulationExchange(TradeMechanismSettings.Default(), _priceService, _clock, _logger);
        _orderListener = new OrderListener(_clock, strategy.PortfolioManager, Result, _logger);
    }

    /// <summary>
    /// Setup all parameters and event listening for the various different parts.
    /// </summary>
    public void Initialise()
    {
        _exchange.Initialize(_settings);
        _priceService.Initialize(_settings);
        _strategy.Initialize(_settings);
        _simulationExchange.Initialize(_settings);
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
        _exchange.Shutdown();
        _priceService.Shutdown();
        _strategy.Shutdown();
        _simulationExchange.Shutdown();
        _orderListener.Shutdown();
        Result.Portfolio = _strategy.PortfolioManager.Portfolio;
        IsActive = false;
    }
}