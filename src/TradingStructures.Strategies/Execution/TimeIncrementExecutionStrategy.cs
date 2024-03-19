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
    private readonly IStockExchange _stockExchange;
    private readonly IDecisionSystem _decisionSystem;
    private TradeCollection? _tradeCollection;

    public string Name => nameof(TimeIncrementExecutionStrategy);

    public TimeIncrementExecutionStrategy(
        IReportLogger logger,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem)
    {
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
        _logger.Log(ReportType.Information, "PriceService", $"{eventArgs.Time:yyyy-MM-ddTHH:mm:ss} - Price for {eventArgs.Instrument.Ticker} has changed to {eventArgs.Price}");
    }

    public void OnExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs)
    {
        _logger.Log(ReportType.Information, "ExchangeService", $"{eventArgs.Time:yyyy-MM-ddTHH:mm:ss} - Exchange session changed from {eventArgs.PreviousSession} to {eventArgs.NewSession}");
        var newSession = eventArgs.NewSession;
        if (newSession == ExchangeSession.Continuous)
        {
            MarketOpen(eventArgs.Time);
        }
        else if (newSession == ExchangeSession.Closed)
        {
            MarketClose(eventArgs.Time);
        }
    }

    private void MarketOpen(DateTime time)
    {
        if (_tradeCollection == null)
        {
            _logger.Log(ReportType.Information, "MarketOpen", $"{time:yyyy-MM-ddTHH:mm:ss} - No Trades to enact");
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

    private void MarketClose(DateTime time) =>
        // Decide which stocks to buy, sell or do nothing with.
        _tradeCollection  = _decisionSystem.Decide(time, _stockExchange, _logger);

    public void Shutdown() { }
}