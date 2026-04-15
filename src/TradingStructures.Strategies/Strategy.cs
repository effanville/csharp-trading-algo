using System;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.MathLibrary.Finance;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.OrderManagement;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;

using Microsoft.Extensions.DependencyInjection;

namespace Effanville.TradingStructures.Strategies;

public class Strategy : IStrategy
{
    private IPriceService? _priceService;
    private IClock? _clock;
    private readonly IReportLogger _logger;
    private readonly IExecutionStrategy _executionStrategy;
    private readonly IPortfolioManager _portfolioManager;
    public string Name => nameof(Strategy);

    /// <summary>
    /// Event to subscribe to for the dealing with Trades created.
    /// </summary>
    public event EventHandler<TradeSubmittedEventArgs>? SubmitTradeEvent;

    public StrategyHistory History { get; }


    public Strategy(
        IExecutionStrategy executionStrategy,
        IPortfolioManager portfolioManager,
        IReportLogger logger)
    {
        _logger = logger;
        _executionStrategy = executionStrategy;
        _portfolioManager = portfolioManager;
        History = new StrategyHistory(_portfolioManager.Portfolio);
        _executionStrategy.SubmitTradeEvent += ExecutionStrategyOnSubmitTradeEvent;
    }

    public bool RegisterServices(IServiceProvider serviceProvider)
    {
        _clock = serviceProvider.GetService<IClock>();
        _priceService = serviceProvider.GetService<IPriceService>();
        return true;
    }

    public void Initialize(EvolverSettings settings)
    {
        _executionStrategy.Initialize(settings);
        _portfolioManager.Initialize(settings);
    }

    private void ExecutionStrategyOnSubmitTradeEvent(object? sender, TradeSubmittedEventArgs e)
    {
        DateTime time = _clock?.UtcNow() ?? default;
        e.Time = time;
        var trade = e.RequestedTrade;
        Trade? validatedTrade = _portfolioManager.ValidateTrade(e.Time, trade, _priceService);
        if (validatedTrade == null)
        {
            _logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Trade {trade} was not valid.");
            return;
        }

        decimal availableFunds = _portfolioManager.AvailableFunds(e.Time);
        if (availableFunds <= 0.0m)
        {
            _logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - No available funds.");
            return;
        }

        e.AvailableFunds = availableFunds;
        e.RequestedTrade = validatedTrade;
        SubmitTradeEvent?.Invoke(sender, e);
    }

    public void Shutdown()
    {
        _executionStrategy.Shutdown();
        _portfolioManager.Shutdown();
        DateTime time = _clock?.UtcNow() ?? default;
        decimal latestValue = _portfolioManager.Portfolio.TotalValue(Totals.All, time);
        DateTime earliestTime = _portfolioManager.Portfolio.FirstValueDate(Totals.All);
        decimal startValue = _portfolioManager.Portfolio.TotalValue(Totals.All, earliestTime);

        DateTime latestTime = _portfolioManager.Portfolio.LatestDate(Totals.All);
        double car = FinanceFunctions.CAR(new DailyValuation(earliestTime, startValue), new DailyValuation(latestTime, latestValue));
        _logger.Info("Ending", $"{time:yyyy-MM-ddTHH:mm:ss} total value {latestValue:C2}");
        _logger.Info("Ending", $"{time:yyyy-MM-ddTHH:mm:ss} total CAR {car}");
    }

    public void OnTimeIncrementUpdate(object? obj, TimeIncrementEventArgs eventArgs)
    {
        _executionStrategy.OnTimeIncrementUpdate(obj, eventArgs);
        _portfolioManager.ReportStatus(eventArgs.Time);
    }

    public void OnExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs)
        => _executionStrategy.OnExchangeStatusChanged(obj, eventArgs);

    public void OnPriceUpdate(object? obj, PriceUpdateEventArgs eventArgs)
    {
        _executionStrategy.OnPriceUpdate(obj, eventArgs);
        _portfolioManager.OnPriceUpdate(obj, eventArgs);
    }

    public void OnTradeConfirmed(object? obj, TradeCompletedEventArgs eventArgs)
    {
        var time = _clock!.UtcNow();
        if (eventArgs.TradeSuccessful)
        {
            Trade trade = eventArgs.RequestedTrade;
            var tradeConfirmation = eventArgs.ConfirmedTrade;
            _logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Confirm trade '{tradeConfirmation}' reported and added.");
            _ = _portfolioManager.AddTrade(time, trade, tradeConfirmation);
            History.Trades.Add(time, trade);
            History.Decisions.Add(time, trade);
        }
        else
        {
            _logger.Log(ReportType.Warning, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Requested trade '{eventArgs.RequestedTrade}' not successful.");
        }
    }
}