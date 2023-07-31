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
    readonly ServiceManager _serviceManager = new ServiceManager();
    IPortfolioManager PortfolioManager => _serviceManager.GetService<IPortfolioManager>(nameof(IPortfolioManager));
    IPriceService PriceService => _serviceManager.GetService<IPriceService>(nameof(IPriceService));
    TradingExchange Exchange => _serviceManager.GetService<TradingExchange>(nameof(TradingExchange));
    ITradeSubmitter TradeSubmitter => _serviceManager.GetService<ITradeSubmitter>(nameof(ITradeSubmitter));
    IExecutionStrategy ExecutionStrategy => _serviceManager.GetService<IExecutionStrategy>(nameof(IExecutionStrategy));

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

        var tradingExchange = new TradingExchange(_scheduler, exchange);
        _serviceManager.RegisterService(nameof(TradingExchange), tradingExchange);

        var tradeSubmitter = TradeSubmitterFactory.Create(TradeSubmitterType.SellAllThenBuy, TradeMechanismSettings.Default());
        _serviceManager.RegisterService(nameof(ITradeSubmitter), tradeSubmitter);

        var priceService = PriceServiceFactory.Create(PriceType.RandomWobble, PriceCalculationSettings.Default(), exchange, _scheduler);
        _serviceManager.RegisterService(nameof(IPriceService), priceService);

        IStockExchange baseStockExchange = StockExchangeFactory.Create(exchange, DateTime.MinValue);
        var executionStrategy = ExecutionStrategyFactory.Create(strategyType, Clock, logger, baseStockExchange, decisionSystem);
        _serviceManager.RegisterService(nameof(IExecutionStrategy), executionStrategy);

        _serviceManager.RegisterService(nameof(IPortfolioManager), portfolioManager);
    }

    /// <summary>
    /// Setup all parameters and event listening for the various different parts.
    /// </summary>
    public void Initialise()
    {
        _serviceManager.Initialize(_settings);

        ExecutionStrategy.SubmitTradeEvent += OnTradeRequested;
        Exchange.ExchangeStatusChanged += ExecutionStrategy.OnExchangeStatusChanged;
        PriceService.PriceChanged += ExecutionStrategy.OnPriceUpdate;
        PriceService.PriceChanged += PortfolioManager.OnPriceUpdate;
        ScheduleShutdown();
        _scheduler.ScheduleNewEvent(TimeUpdate, Clock.UtcNow().AddDays(1));
        _isInitialised = true;
        _logger.Log(ReportType.Information, nameof(EventEvolver), "Inititalization complete");
    }

    public void TimeUpdate()
    {
        var time = Clock.UtcNow();
        _ = Task.Run(() => ExecutionStrategy.OnTimeIncrementUpdate(null, new TimeIncrementEventArgs(time)));
        _ = Task.Run(() => PortfolioManager.ReportStatus(time));
        _scheduler.ScheduleNewEvent(TimeUpdate, time.AddDays(1));
    }

    private void OnTradeRequested(object obj, TradeSubmittedEventArgs eventArgs)
        => TradeSubmitterHelpers.SubmitAndReportTrade(
            Clock.UtcNow(),
            eventArgs.RequestedTrade,
            PriceService,
            PortfolioManager,
            TradeSubmitter,
            Result.Trades,
            Result.Decisions,
            _logger);

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
        Clock.Start();
    }

    /// <summary>
    /// End everything from running and record the results.
    /// </summary>
    public void Shutdown()
    {
        _scheduler.Stop();
        _serviceManager.Shutdown();
        Result.Portfolio = PortfolioManager.Portfolio;
        DateTime time = Clock.UtcNow();
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time} total value {PortfolioManager.Portfolio.TotalValue(Totals.All):C2}");
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time} total CAR {PortfolioManager.Portfolio.TotalIRR(Totals.All)}");
        IsActive = false;
    }
}