using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Effanville.FinancialStructures.NamingStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Scheduling;
using Effanville.TradingStructures.Common.Services;
using Effanville.TradingStructures.Common.Time;

using Nager.Date;

namespace Effanville.TradingStructures.Exchanges;

/// <summary>
/// Representation of a exchange and all instruments that are traded upon it.
/// </summary>
public sealed class TradingExchange : IService
{
    private readonly IScheduler _scheduler;

    public string Name { get; private set; }

    /// <summary>
    /// The code for the country to determine trading days.
    /// </summary>
    private CountryCode CountryDateCode { get; set; }

    [XmlIgnore] private readonly Dictionary<string, NameData> StockInstruments = new Dictionary<string, NameData>();

    private static TimeOnly ExchangeOpen { get; set; }
    private static TimeOnly ExchangeClose { get; set; }

    public event EventHandler<ExchangeStatusChangedEventArgs>? ExchangeStatusChanged;

    public TradingExchange(IScheduler scheduler, IStockExchange stockExchange)
    {
        Name = stockExchange.Name;
        _scheduler = scheduler;
        Configure(stockExchange);
    }

    public void Initialize(EvolverSettings settings)
    {
        DateTime time = settings.StartTime.Date;
        while (settings.StartTime <= time && time < settings.EndTime)
        {
            InitialiseDayEvents(time);
            time = time.AddDays(1);
        }
    }

    private void InitialiseDayEvents(DateTime time)
    {
        if (!DateHelpers.IsCalcTimeValid(time, CountryDateCode))
        {
            return;
        }

        _scheduler.ScheduleNewEvent(
            () => RaiseExchangeStatusChanged(null, new ExchangeStatusChangedEventArgs(ExchangeSession.Closed, ExchangeSession.Continuous)),
            time.Date.Add(ExchangeOpen.ToTimeSpan()));
        _scheduler.ScheduleNewEvent(
            () => RaiseExchangeStatusChanged(null, new ExchangeStatusChangedEventArgs(ExchangeSession.Continuous, ExchangeSession.Closed)),
            time.Date.Add(ExchangeClose.ToTimeSpan()));
    }

    public void Restart() { }
    public void Shutdown() { }

    private void RaiseExchangeStatusChanged(object? obj, ExchangeStatusChangedEventArgs eventArgs)
    {
        EventHandler<ExchangeStatusChangedEventArgs>? handler = ExchangeStatusChanged;
        handler?.Invoke(obj, eventArgs);
    }

    private void Configure(IStockExchange stockExchange)
    {
        Name = stockExchange.Name;
        ExchangeOpen = stockExchange.ExchangeOpen;
        ExchangeClose = stockExchange.ExchangeClose;
        CountryDateCode = stockExchange.CountryDateCode;
        foreach (var stock in stockExchange.Stocks)
        {
            string ticker = stock.Name.Ticker;
            StockInstruments.Add(ticker, stock.Name);
        }
    }
}