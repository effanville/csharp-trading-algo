using System;
using System.Collections.Generic;

using Effanville.FinancialStructures.DataStructures;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.StockMarket;

namespace Effanville.TradingStructures.OrderManagement;

public sealed class OrderManagementService : IOrderManagementService
{
    private readonly IClock _clock;
    private readonly IStockMarketAdapter _stockMarketAdapter;
    private readonly OrderManagementSettings _settings;
    private readonly Dictionary<Guid, TradeSubmittedEventArgs> _pendingTrades = new Dictionary<Guid, TradeSubmittedEventArgs>();

    public string Name => nameof(OrderManagementService);

    public event EventHandler<TradeCompletedEventArgs>? TradeCompleted;
    public OrderManagementService(
        IClock clock,
        IStockMarketAdapter stockMarketAdapter,
        OrderManagementSettings settings)
    {
        _clock = clock;
        _stockMarketAdapter = stockMarketAdapter;
        _settings = settings;
        _stockMarketAdapter.TradeCompleted += OnTradeConfirmed;
    }

    public void Initialize(EvolverSettings settings) { }
    public void OnTradeRequested(object? obj, TradeSubmittedEventArgs eventArgs)
    {
        DateTime time = _clock?.UtcNow() ?? default;
        eventArgs.SendTime = time;
        Guid id = Guid.NewGuid();
        // should do something like order sanity checks here.

        Trade trade = eventArgs.RequestedTrade;
        decimal availableFunds = eventArgs.AvailableFunds;

        decimal sign = trade.BuySell.Sign();
        bool isInvestmentAltering = trade.BuySell.IsInvestmentTradeType();
        decimal totalCost = isInvestmentAltering ? trade.NumberShares * trade.LimitPrice + sign * _settings.TradeCost : 0.0m;
        if (trade.BuySell == TradeType.Buy
            && totalCost > availableFunds)
        {
            return;
        }
        _pendingTrades.Add(id, eventArgs);
        _stockMarketAdapter.OnTradeRequested(obj, new TradeRequestEventArgs(id, trade));
    }

    public void OnTradeConfirmed(object? obj, TradeResponseEventArgs eventArgs)
    {
        DateTime time = _clock?.UtcNow() ?? default;
        _pendingTrades.TryGetValue(eventArgs.Id, out var requestedTrade);
        _pendingTrades.Remove(eventArgs.Id);
        TradeCompleted?.Invoke(obj, new TradeCompletedEventArgs(
            eventArgs.Id,
            time,
            eventArgs.RequestedTrade,
            eventArgs.ConfirmedTrade,
            eventArgs.TradeSuccessful));
    }
}
