using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Xml.Serialization;

using Common.Structure.Reporting;

using FinancialStructures.StockStructures;

using Nager.Date;

using TradingSystem.MarketEvolvers;
using TradingSystem.Time;

namespace TradingSystem.ExchangeStructures;

/// <summary>
/// Representation of a exchange and all instruments that are traded upon it.
/// </summary>
public sealed class TradingExchange
{
    private readonly Scheduler _scheduler;

    public string Name
    {
        get; set;
    }

    public TimeZoneInfo TimeZone
    {
        get; set;
    }

    /// <summary>
    /// The code for the country to determine trading days.
    /// </summary>
    public CountryCode CountryDateCode
    {
        get;
        private set;
    } = CountryCode.GB;

    [XmlIgnore]
    public Dictionary<string, StockInstrument> StockInstruments;

    public static TimeOnly ExchangeOpen => new TimeOnly(8, 0, 0);
    public static TimeOnly ExchangeClose => new TimeOnly(16, 30, 0);

    public event EventHandler<ExchangeStatusChangedEventArgs> ExchangeStatusChanged;

    public TradingExchange()
    {
        StockInstruments = new Dictionary<string, StockInstrument>();
    }

    public TradingExchange(Scheduler scheduler)
    : this()
    {
        _scheduler = scheduler;
    }

    public TradingExchange(Scheduler scheduler, IStockExchange stockExchange)
        : this(scheduler)
    {
        Configure(stockExchange);
    }

    public void Initialise(EvolverSettings settings)
    {
        DateTime time = settings.StartTime;
        while (time < settings.EndTime)
        {
            InitialiseDayEvents(time);
            time = time.AddDays(1);
        }
    }

    public void InitialiseDayEvents(DateTime time)
    {
        if (DateHelpers.IsCalcTimeValid(time, CountryDateCode))
        {
            _scheduler.ScheduleNewEvent(
                () => RaiseExchangeStatusChanged(null, new ExchangeStatusChangedEventArgs(ExchangeSession.Closed, ExchangeSession.Continuous)),
                new DateTime(time.Year, time.Month, time.Day, ExchangeOpen.Hour, ExchangeOpen.Minute, ExchangeOpen.Second));
            _scheduler.ScheduleNewEvent(
                () => RaiseExchangeStatusChanged(null, new ExchangeStatusChangedEventArgs(ExchangeSession.Continuous, ExchangeSession.Closed)),
                new DateTime(time.Year, time.Month, time.Day, ExchangeClose.Hour, ExchangeClose.Minute, ExchangeClose.Second));
        }
    }

    private void RaiseExchangeStatusChanged(object obj, ExchangeStatusChangedEventArgs eventArgs)
    {
        EventHandler<ExchangeStatusChangedEventArgs> handler = ExchangeStatusChanged;
        if (handler != null)
        {
            handler?.Invoke(obj, eventArgs);
        }
    }

    public void Configure(IStockExchange stockExchange)
    {
        foreach (var stock in stockExchange.Stocks)
        {
            string ticker = stock.Ticker;
            StockInstrument stockInst = new StockInstrument(ticker, stock.Name);
            StockInstruments.Add(ticker, stockInst);
        }
    }

    public void Configure(string stockFilePath, IFileSystem fileSystem, IReportLogger logger = null)
    {
        string[] fileContents = Array.Empty<string>();
        try
        {
            fileContents = fileSystem.File.ReadAllLines(stockFilePath);
        }
        catch (Exception ex)
        {
            logger?.Error(ReportLocation.AddingData.ToString(), $"Failed to read from file located at {stockFilePath}: {ex.Message}.");
        }

        if (fileContents.Length == 0)
        {
            logger?.Error(ReportLocation.AddingData.ToString().ToString(), "Nothing in file selected, but expected stock company, name, url data.");
            return;
        }

        foreach (string line in fileContents)
        {
            string[] inputs = line.Split(',');
            AddStock(inputs, logger);
        }

        logger?.Log(ReportType.Information, ReportLocation.AddingData.ToString(), $"Configured StockExchange from file {stockFilePath}.");
    }

    private void AddStock(string[] parameters, IReportLogger logger = null)
    {
        if (parameters.Length != 5)
        {
            logger?.Error(ReportLocation.AddingData.ToString(), "Insufficient Data in line to add Stock");
            return;
        }

        StockInstrument stock = new StockInstrument(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
        StockInstruments.Add(parameters[0], stock);
    }
}