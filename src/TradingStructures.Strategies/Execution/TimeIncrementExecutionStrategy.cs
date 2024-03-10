using System;
using System.Linq;

using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Trading;

namespace Effanville.TradingStructures.Strategies.Execution;

public class TimeIncrementExecutionStrategy : IExecutionStrategy
{
    public event EventHandler<TradeSubmittedEventArgs>? SubmitTradeEvent;
    private readonly IReportLogger _logger;
    private readonly IClock _clock;
    private readonly IStockExchange _stockExchange;
    private readonly IDecisionSystem _decisionSystem;
    private TradeCollection? _tradeCollection;

    public string Name => nameof(TimeIncrementExecutionStrategy);

    public TimeIncrementExecutionStrategy(
        IClock clock,
        IReportLogger logger,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem)
    {
        _clock = clock;
        _logger = logger;
        _stockExchange = stockExchange;
        _decisionSystem = decisionSystem;
    }

    public void Initialize(EvolverSettings settings) { }

    public void Restart() { }

    public void OnTimeIncrementUpdate(object? obj, TimeIncrementEventArgs eventArgs) { }

    public void OnPriceUpdate(object? obj, PriceUpdateEventArgs eventArgs)
    {
        _stockExchange.Stocks.First(stock => stock.Name.Equals(eventArgs.Instrument)).AddValue(eventArgs.Candle);
        _logger.Log(ReportType.Information, "PriceService", $"{_clock.UtcNow():yyyy-MM-ddTHH:mm:ss} - Price for {eventArgs.Instrument.Ticker} has changed to {eventArgs.Price}");
    }

    public void OnExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs)
    {
        _logger.Log(ReportType.Information, "ExchangeService", $"{_clock.UtcNow():yyyy-MM-ddTHH:mm:ss} - Exchange session changed from {eventArgs.PreviousSession} to {eventArgs.NewSession}");
        var newSession = eventArgs.NewSession;
        if (newSession == ExchangeSession.Continuous)
        {
            MarketOpen();
        }
        else if (newSession == ExchangeSession.Closed)
        {
            MarketClose();
        }
    }

    private void MarketOpen()
    {
        if (_tradeCollection == null)
        {
            _logger.Log(ReportType.Information, "MarketOpen", $"{_clock.UtcNow():yyyy-MM-ddTHH:mm:ss} - No Trades to enact");
            return;
        }

        foreach (Trade trade in _tradeCollection.GetSellDecisions())
        {
            SubmitTradeEvent?.Invoke(null, new TradeSubmittedEventArgs(trade));
        }

        foreach (Trade trade in _tradeCollection.GetBuyDecisions())
        {
            SubmitTradeEvent?.Invoke(null, new TradeSubmittedEventArgs(trade));
        }

        _tradeCollection = null;
    }

    private void MarketClose()
    {
        // Decide which stocks to buy, sell or do nothing with.
        var time = _clock.UtcNow();
        _tradeCollection  = _decisionSystem.Decide(time, _stockExchange, _logger);
    }

    public void Shutdown() { }
}