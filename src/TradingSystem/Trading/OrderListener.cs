using System;

using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;
using Effanville.TradingSystem.MarketEvolvers;

namespace Effanville.TradingSystem.Trading;

public class OrderListener : IOrderListener
{
    private readonly IClock _clock;
    private readonly IPortfolioManager _portfolioManager;
    private readonly EvolverResult _result;
    private readonly IReportLogger _logger;

    public string Name => nameof(OrderListener);

    public event EventHandler<TradeSubmittedEventArgs>? SubmitTrade; 

    public OrderListener(
        IClock clock,
        IPortfolioManager portfolioManager,
        EvolverResult result,
        IReportLogger logger)
    {
        _clock = clock;
        _portfolioManager = portfolioManager;
        _result = result;
        _logger = logger;
    }

    public void Initialize(EvolverSettings settings)
    {
    }

    public void Restart()
    {
    }

    public void Shutdown()
    {
    }

    public void OnTradeRequested(object? obj, TradeSubmittedEventArgs eventArgs) 
        => SubmitTrade?.Invoke(null,eventArgs);

    public void OnTradeConfirmed(object? obj, TradeCompletedEventArgs eventArgs)
    {
        var time = _clock.UtcNow();
        if (eventArgs.TradeSuccessful)
        {
            var trade = eventArgs.RequestedTrade;
            var tradeConfirmation = eventArgs.ConfirmedTrade;
            _logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Confirm trade '{tradeConfirmation}' reported and added.");
            _ = _portfolioManager.AddTrade(time, trade, tradeConfirmation);
            _result.Trades.Add(time, trade);
            _result.Decisions.Add(time, trade);
        }
        else
        {
            _logger.Log(ReportType.Warning, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Requested trade '{eventArgs.RequestedTrade}' not successful.");
        }
    }
}