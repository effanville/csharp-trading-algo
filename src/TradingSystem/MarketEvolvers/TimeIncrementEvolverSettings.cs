using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;

using Nager.Date;

namespace Effanville.TradingSystem.MarketEvolvers;

/// <summary>
/// Settings required for a simulator to simulate.
/// </summary>
public sealed class TimeIncrementEvolverSettings : EvolverSettings
{
    /// <summary>
    /// The code for the country to determine trading days.
    /// </summary>
    public CountryCode CountryDateCode { get; private set; }

    /// <summary>
    /// The stock exchange to use for this simulation.
    /// </summary>
    public IStockExchange Exchange { get; }

    public TimeIncrementEvolverSettings(DateTime startTime, DateTime endTime, TimeSpan evolutionIncrement, CountryCode countryCode = CountryCode.GB)
        : base(startTime, endTime, evolutionIncrement)
    {
        CountryDateCode = countryCode;
    }
}
