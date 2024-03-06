using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Xml.Serialization;

using Effanville.Common.Structure.Reporting;
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
    public Dictionary<string, NameData> StockInstruments;

    public static TimeOnly ExchangeOpen => new TimeOnly(8, 0, 0);
    public static TimeOnly ExchangeClose => new TimeOnly(16, 30, 0);

    public event EventHandler<ExchangeStatusChangedEventArgs> ExchangeStatusChanged;

    public TradingExchange()
    {
        StockInstruments = new Dictionary<string, NameData>();
    }

    public TradingExchange(IScheduler scheduler)
    : this()
    {
        _scheduler = scheduler;
    }

    public TradingExchange(IScheduler scheduler, IStockExchange stockExchange)
        : this(scheduler)
    {
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

    public void InitialiseDayEvents(DateTime time)
    {
        if (DateHelpers.IsCalcTimeValid(time, CountryDateCode))
        {
            _scheduler.ScheduleNewEvent(
                () => RaiseExchangeStatusChanged(null, new ExchangeStatusChangedEventArgs(ExchangeSession.Closed, ExchangeSession.Continuous)),
                time.Date.Add(ExchangeOpen.ToTimeSpan()));
            _scheduler.ScheduleNewEvent(
                () => RaiseExchangeStatusChanged(null, new ExchangeStatusChangedEventArgs(ExchangeSession.Continuous, ExchangeSession.Closed)),
                time.Date.Add(ExchangeClose.ToTimeSpan()));
        }
    }

    public void Restart() { }
    public void Shutdown() { }

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
            string ticker = stock.Name.Ticker;
            StockInstruments.Add(ticker, stock.Name);
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

        NameData stock =
            new NameData(parameters[1].Trim(), parameters[2].Trim(), parameters[3].Trim(), parameters[4].Trim())
            {
                Ticker = parameters[0]
            };
        StockInstruments.Add(parameters[0], stock);
    }
}