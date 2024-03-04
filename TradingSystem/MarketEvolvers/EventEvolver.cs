using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Stocks;

using TradingSystem.Time;

using System;

using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;
using Effanville.TradingSystem.Trading;

namespace TradingSystem.MarketEvolvers;

/// <summary>
/// An evolver for a Stock market that is based on events being raised. Either
/// Exchange change events or price changed events (or others).
/// </summary>
public sealed class EventEvolver
{
    bool _isInitialised;
    private readonly IClock _clock;
    readonly EvolverSettings _settings;
    readonly IReportLogger _logger;
    readonly IScheduler _scheduler;
    readonly ServiceManager _serviceManager = new ServiceManager();
    IPriceService PriceService => _serviceManager.GetService<IPriceService>(nameof(IPriceService));
    TradingExchange Exchange => _serviceManager.GetService<TradingExchange>(nameof(TradingExchange));
    private IMarketExchange SimulationExchange => _serviceManager.GetService<IMarketExchange>(nameof(IMarketExchange));
    IOrderListener OrderListener => _serviceManager.GetService<IOrderListener>(nameof(IOrderListener));
    private IStrategy Strategy => _serviceManager.GetService<IStrategy>(nameof(IStrategy));

    /// <summary>
    /// Whether this evolution is still running.
    /// </summary>
    public bool IsActive
    {
        get; private set;
    }

    public EvolverResult Result
    {
        get; private set;
    } = new EvolverResult();

    public EventEvolver(
        EvolverSettings settings,
        IStockExchange exchange,
        IPortfolioManager portfolioManager,
        StrategyType strategyType,
        IDecisionSystem decisionSystem,
        IReportLogger logger)
    {
        _settings = settings;
        _logger = logger;
        _clock = new SimulationEventBasedClock(settings.StartTime);
        _scheduler = new Scheduler(_clock);

        var tradingExchange = new TradingExchange(_scheduler, exchange);
        _serviceManager.RegisterService(nameof(TradingExchange), tradingExchange);

        var priceService = PriceServiceFactory.Create(PriceType.RandomWobble, PriceCalculationSettings.Default(), exchange, _scheduler);
        _serviceManager.RegisterService(nameof(IPriceService), priceService);
        var executionStrategy = ExecutionStrategyFactory.Create(strategyType, _clock, logger, exchange, decisionSystem);
        var strategy = new Strategy(decisionSystem, executionStrategy, portfolioManager, priceService, _clock, _logger);
        _serviceManager.RegisterService(nameof(IStrategy), strategy);
        
        var simulationExchange = new SimulationExchange(TradeMechanismSettings.Default(), priceService, _clock, _logger);
        _serviceManager.RegisterService(nameof(IMarketExchange), simulationExchange);
        var orderListener = new OrderListener(_clock, portfolioManager, Result, _logger);
        _serviceManager.RegisterService(nameof(IOrderListener), orderListener);
    }

    /// <summary>
    /// Setup all parameters and event listening for the various different parts.
    /// </summary>
    public void Initialise()
    {
        _serviceManager.Initialize(_settings);
        Strategy.SubmitTradeEvent += OrderListener.OnTradeRequested;
        Exchange.ExchangeStatusChanged += Strategy.OnExchangeStatusChanged;
        PriceService.PriceChanged += Strategy.OnPriceUpdate;

        OrderListener.SubmitTrade += SimulationExchange.OnTradeRequested;
        SimulationExchange.TradeCompleted += OrderListener.OnTradeConfirmed;
        ScheduleShutdown();
        _scheduler.ScheduleNewEvent(TimeUpdate, _clock.UtcNow().AddDays(1));
        _isInitialised = true;
        _logger.Log(ReportType.Information, nameof(EventEvolver), "Initialization complete");
    }

    public void TimeUpdate()
    {
        var time = _clock.UtcNow();
        Strategy.OnTimeIncrementUpdate(null, new TimeIncrementEventArgs(time));
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
        _scheduler.Stop();
        _serviceManager.Shutdown();
        Result.Portfolio = Strategy.PortfolioManager.Portfolio;
        IsActive = false;
    }
}