using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;

using TradingSystem.MarketEvolvers;
using TradingSystem.Simulation;

namespace Effanville.TradingSystem.Trading;

public class OrderListener : IOrderListener
{
    private readonly IClock _clock;
    private readonly IMarketExchange _marketExchange;
    private readonly IPriceService _priceService;
    private readonly IPortfolioManager _portfolioManager;
    private readonly ITradeSubmitter _tradeSubmitter;
    private readonly EvolverResult _result;
    private readonly IReportLogger _logger;

    public string Name => nameof(OrderListener);

    public OrderListener(
        IClock clock,
        IMarketExchange marketExchange,
        IPriceService priceService,
        IPortfolioManager portfolioManager,
        ITradeSubmitter tradeSubmitter,
        EvolverResult result,
        IReportLogger logger)
    {
        _clock = clock;
        _marketExchange = marketExchange;
        _priceService = priceService;
        _portfolioManager = portfolioManager;
        _tradeSubmitter = tradeSubmitter;
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

    public void OnTradeRequested(object obj, TradeSubmittedEventArgs eventArgs)
    {
        var time = _clock.UtcNow();
        var trade = eventArgs.RequestedTrade;
        decimal availableFunds = _portfolioManager.AvailableFunds(time);
        var tradeConfirmation = _tradeSubmitter.Trade(time, trade, _priceService, availableFunds, _logger);
        if (tradeConfirmation != null)
        {
            _logger.Log(ReportType.Information, "Trading", $"{time:yyyy-MM-ddTHH:mm:ss} - Confirm trade '{tradeConfirmation}' reported and added.");
            _ = _portfolioManager.AddTrade(time, trade, tradeConfirmation);
            _result.Trades.Add(time, trade);
        }
        _result.Decisions.Add(time, trade);
    }
}