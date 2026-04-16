using System;
using System.Linq;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Implementation;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Exchanges;
using Effanville.TradingStructures.Pricing;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Trading;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Execution;

public class ExchangeOpenCalcExecutionStrategy : IExecutionStrategy
{
    public event EventHandler<TradeSubmittedEventArgs>? SubmitTradeEvent;
    private readonly ILogger<ExchangeOpenCalcExecutionStrategy> _logger;
    private readonly IStockExchange _stockExchange;
    private readonly IDecisionSystem _decisionSystem;
    private TradeCollection? _tradeCollection;
    private bool _calibrated;

    public string Name => nameof(ExchangeOpenCalcExecutionStrategy);

    public ExchangeOpenCalcExecutionStrategy(
        ILogger<ExchangeOpenCalcExecutionStrategy> logger,
        IStockExchange stockExchange,
        IDecisionSystem decisionSystem)
    {
        _logger = logger;
        _stockExchange = StockExchangeFactory.Create(stockExchange, DateTime.MinValue);
        _decisionSystem = decisionSystem;
    }

    public void Initialize(EvolverSettings settings) { }

    public void Restart() { }

    public void OnTimeIncrementUpdate(object? obj, TimeIncrementEventArgs eventArgs) { }

    public void OnPriceUpdate(object? obj, PriceUpdateEventArgs eventArgs)
    {
        Stock? stock = _stockExchange.Stocks.FirstOrDefault(stock => stock.Name.Equals(eventArgs.Instrument));
        if (stock == null)
        {
            NameData name = eventArgs.Instrument;
            stock = new Stock(name.Ticker, name.Company, name.Name, name.Currency, name.Url);
            _stockExchange.Stocks.Add(stock);
        }
        if (stock.Valuations.Any(x => x.Start == eventArgs.Candle.Start))
        {
            stock.Valuations.RemoveAll(x => x.Start == eventArgs.Candle.Start);
        }
        stock.AddValue(eventArgs.Candle);
        _logger.LogInformation($"Update. Stock={eventArgs.Instrument.Ticker}, Time={eventArgs.Time:yyyy-MM-ddTHH:mm:ss}, Price={eventArgs.Price}");
        int numberVals = _stockExchange.NumberValuations();
        if (!_calibrated && _decisionSystem.MinBurnInPeriod < _stockExchange.NumberValuations())
        {
            var settings = new DecisionSystemSettings(
                    _stockExchange.StartDate(),
                    eventArgs.Time,
                    _stockExchange.Stocks.Count,
                    _stockExchange);
            _decisionSystem.Calibrate(settings);
            _calibrated = true;
        }
    }

    public void OnExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs)
    {
        _logger.LogInformation($"SessionChange. Time={eventArgs.Time:yyyy-MM-ddTHH:mm:ss}, Old={eventArgs.PreviousSession}, New={eventArgs.NewSession}");
        var newSession = eventArgs.NewSession;
        if (newSession == ExchangeSession.Continuous)
        {
            MarketClose(eventArgs.Time);
            MarketOpen(eventArgs.Time);
        }
        else if (newSession == ExchangeSession.Closed)
        {
        }
    }

    private void MarketOpen(DateTime time)
    {
        if (_tradeCollection == null)
        {
            _logger.LogInformation($"{time:yyyy-MM-ddTHH:mm:ss} - No Trades to enact");
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
        _tradeCollection = _decisionSystem.Decide(time, _stockExchange);

    public void Shutdown() { }
}