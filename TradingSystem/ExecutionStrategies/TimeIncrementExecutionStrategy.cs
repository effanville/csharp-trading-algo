using System;
using System.Linq;

using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;

using TradingSystem.Decisions;
using TradingSystem.MarketEvolvers;
using TradingSystem.Time;
using TradingSystem.Trading;

namespace TradingSystem.ExecutionStrategies;

public class TimeIncrementExecutionStrategy : IExecutionStrategy
{
    public event EventHandler<TradeSubmittedEventArgs> SubmitTradeEvent;
    private readonly IReportLogger _logger;
    private readonly IClock _clock;
    private readonly IStockExchange _stockExchange;
    private readonly IDecisionSystem _decisionSystem;
    private TradeCollection _tradeCollection;

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

    public void Initialize(EvolverSettings settings)
    {
    }

    public void Restart() => throw new NotImplementedException();
    public void OnTimeIncrementUpdate(object obj, TimeIncrementEventArgs eventArgs)
    {
    }

    public void OnPriceUpdate(object obj, PriceUpdateEventArgs eventArgs)
    {
        _stockExchange.Stocks.First(stock => stock.Name.Equals(eventArgs.Instrument.Name)).AddValue(eventArgs.Candle);
        _logger.Log(ReportType.Information, "PriceService", $"{_clock.UtcNow()} - Price for {eventArgs.Instrument.Name.Ticker} has changed to {eventArgs.Price}");
    }

    public void OnExchangeStatusChanged(object obj, ExchangeStatusChangedEventArgs eventArgs)
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
        TradeCollection requestedTrades = _decisionSystem.Decide(time, _stockExchange, _logger);
        _tradeCollection = requestedTrades;
    }

    public void Shutdown()
    {
    }
}