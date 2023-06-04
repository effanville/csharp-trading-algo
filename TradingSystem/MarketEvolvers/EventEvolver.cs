using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.StockStructures;

using TradingSystem.Decisions;
using TradingSystem.ExchangeStructures;
using TradingSystem.ExecutionStrategies;
using TradingSystem.PriceSystem;
using TradingSystem.Trading;
using TradingSystem.Time;
using TradingSystem.PortfolioStrategies;
using System;
using System.Threading.Tasks;
using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.Database.Extensions.Rates;

namespace TradingSystem.MarketEvolvers;

/// <summary>
/// An evolver for a Stock market that is based on events being raised. Either
/// Exchange change events or price changed events (or others).
/// </summary>
public sealed partial class EventEvolver
{
    bool _isInitialised;
    readonly EvolverSettings _settings;
    readonly IReportLogger _logger;
    readonly Scheduler _scheduler;
    readonly IPriceService _priceService;
    readonly ITradeSubmitter _tradeSubmitter;
    readonly TradingExchange _exchange;
    readonly IExecutionStrategy _executionStrategy;
    readonly IPortfolioManager _portfolioManager;

    /// <summary>
    /// Contains the timing mechanism in the evolver.
    /// </summary>
    public IClock Clock
    {
        get;
    }

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
        Clock = new SimulationClock(settings.StartTime);
        _scheduler = new Scheduler(Clock);

        _exchange = new TradingExchange(_scheduler, exchange);
        _tradeSubmitter = TradeSubmitterFactory.Create(TradeSubmitterType.SellAllThenBuy, TradeMechanismSettings.Default());
        _priceService = PriceServiceFactory.Create(PriceType.RandomWobble, PriceCalculationSettings.Default(), exchange, _scheduler);

        IStockExchange baseStockExchange = StockExchangeFactory.Create(exchange, DateTime.MinValue);
        _portfolioManager = portfolioManager;
        _executionStrategy = ExecutionStrategyFactory.Create(strategyType, Clock, logger, baseStockExchange, decisionSystem);
    }

    /// <summary>
    /// Setup all parameters and event listening for the various different parts.
    /// </summary>
    public void Initialise()
    {
        _exchange.Initialise(_settings);
        _priceService.Initialise(_settings.StartTime, _settings.EndTime);

        _executionStrategy.Initialise();
        _executionStrategy.SubmitTradeEvent += OnTradeRequested;
        _exchange.ExchangeStatusChanged += _executionStrategy.OnExchangeStatusChanged;
        _priceService.PriceChanged += _executionStrategy.OnPriceUpdate;
        _priceService.PriceChanged += _portfolioManager.OnPriceUpdate;
        ScheduleShutdown();
        _scheduler.ScheduleNewEvent(TimeUpdate, Clock.UtcNow().AddDays(1));
        _isInitialised = true;
        _logger.Log(ReportType.Information, "EventEvolver", "Inititalization complete");
    }

    public void TimeUpdate()
    {
        var time = Clock.UtcNow();
        _ = Task.Run(() => _executionStrategy.OnTimeIncrementUpdate(null, new TimeIncrementEventArgs(time)));
        _ = Task.Run(() => _portfolioManager.ReportStatus(time));
        _scheduler.ScheduleNewEvent(TimeUpdate, time.AddDays(1));
    }

    private void OnTradeRequested(object obj, TradeSubmittedEventArgs eventArgs)
        => TradeSubmitterHelpers.SubmitAndReportTrade(
            Clock.UtcNow(),
            eventArgs.RequestedTrade,
            _priceService,
            _portfolioManager,
            _tradeSubmitter,
            Result.Trades,
            Result.Decisions,
            _logger);

    private void ScheduleShutdown() => _scheduler.ScheduleNewEvent(End, _settings.EndTime);

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
        Clock.Start();
    }

    /// <summary>
    /// End everything from running and record the results.
    /// </summary>
    public void End()
    {
        _executionStrategy.Shutdown();
        Result.Portfolio = _portfolioManager.Portfolio;
        DateTime time = Clock.UtcNow();
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time} total value {_portfolioManager.Portfolio.TotalValue(Totals.All):C2}");
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time} total CAR {_portfolioManager.Portfolio.TotalIRR(Totals.All)}");
        IsActive = false;
    }
}