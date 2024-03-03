using Effanville.Common.Structure.Reporting;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Trading;

using TradingSystem.MarketEvolvers;
using TradingSystem.Simulation;
using TradingSystem.Trading;

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
        => TradeSubmitterHelpers.SubmitAndReportTrade(
            _clock.UtcNow(),
            eventArgs.RequestedTrade,
            _priceService,
            _portfolioManager,
            _tradeSubmitter,
            _result.Trades,
            _result.Decisions,
            _logger);
}