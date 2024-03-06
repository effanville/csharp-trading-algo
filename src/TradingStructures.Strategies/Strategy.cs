using System;
using System.Threading.Tasks;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.MathLibrary.Finance;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingStructures.Strategies;

public class Strategy : IStrategy
{
    private readonly IPriceService _priceService;
    private readonly IClock _clock;
    private readonly IReportLogger _logger;
    public string Name => nameof(Strategy);
    
    /// <summary>
    /// Event to subscribe to for the dealing with Trades created.
    /// </summary>
    public event EventHandler<TradeSubmittedEventArgs> SubmitTradeEvent;
    public IDecisionSystem DecisionSystem { get; }
    public IExecutionStrategy ExecutionStrategy { get; }
    public IPortfolioManager PortfolioManager { get; }

    public Strategy(
        IDecisionSystem decisionSystem, 
        IExecutionStrategy executionStrategy,
        IPortfolioManager portfolioManager,
        IPriceService priceService,
        IClock clock,
        IReportLogger logger)
    {
        _priceService = priceService;
        _clock = clock;
        _logger = logger;
        DecisionSystem = decisionSystem;
        ExecutionStrategy = executionStrategy;
        PortfolioManager = portfolioManager;
    }

    public void Initialize(EvolverSettings settings)
    
    {
        ExecutionStrategy.Initialize(settings);
        ExecutionStrategy.SubmitTradeEvent += ExecutionStrategyOnSubmitTradeEvent;
        PortfolioManager.Initialize(settings);
    }

    private void ExecutionStrategyOnSubmitTradeEvent(object sender, TradeSubmittedEventArgs e)
    {
        var time = _clock.UtcNow();
        var trade = e.RequestedTrade;
        Trade validatedTrade = PortfolioManager.ValidateTrade(time, trade, _priceService);
        if (validatedTrade == null)
        {
            _logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Trade {trade} was not valid.");
            return;
        }

        decimal availableFunds = PortfolioManager.AvailableFunds(time);
        if (availableFunds <= 0.0m)
        {
            _logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - No available funds.");
            return;
        }

        e.AvailableFunds = availableFunds;
        e.RequestedTrade = validatedTrade;
        SubmitTradeEvent?.Invoke(sender, e);
    }

    public void Restart()
    {
        ExecutionStrategy.Restart();
        PortfolioManager.Restart();
    }

    public void Shutdown()
    {
        ExecutionStrategy.Shutdown();
        PortfolioManager.Shutdown();
        DateTime time = _clock.UtcNow();
        decimal latestValue = PortfolioManager.Portfolio.TotalValue(Totals.All, time);
        DateTime earliestTime = PortfolioManager.Portfolio.FirstValueDate(Totals.All);
        decimal startValue = PortfolioManager.Portfolio.TotalValue(Totals.All, earliestTime);

        DateTime latestTime = PortfolioManager.Portfolio.LatestDate(Totals.All);
        double car = FinanceFunctions.CAR(new DailyValuation(earliestTime, startValue), new DailyValuation(latestTime, latestValue));
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time:yyyy-MM-ddTHH:mm:ss} total value {latestValue:C2}");
        _logger.Log(ReportSeverity.Critical, ReportType.Information, "Ending", $"{time:yyyy-MM-ddTHH:mm:ss} total CAR {car}");
    }

    public void OnTimeIncrementUpdate(object obj, TimeIncrementEventArgs eventArgs)
    {
        ExecutionStrategy.OnTimeIncrementUpdate(obj, eventArgs);
        PortfolioManager.ReportStatus(eventArgs.Time);
    }

    public void OnExchangeStatusChanged(object obj, ExchangeStatusChangedEventArgs eventArgs) 
        => ExecutionStrategy.OnExchangeStatusChanged(obj, eventArgs);

    public void OnPriceUpdate(object obj, PriceUpdateEventArgs eventArgs)
    {
        ExecutionStrategy.OnPriceUpdate(obj, eventArgs);
        PortfolioManager.OnPriceUpdate(obj, eventArgs);
    }
}