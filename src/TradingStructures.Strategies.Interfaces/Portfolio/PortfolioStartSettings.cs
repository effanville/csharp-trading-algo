using System;

using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.TradingStructures.Strategies.Portfolio;

/// <summary>
/// Settings for constructing the start portfolio.
/// </summary>
public class PortfolioStartSettings
{
    public const string OptionsName = nameof(PortfolioStartSettings);

    /// <summary>
    /// The filepath for the start portfolio.
    /// </summary>
    public string? PortfolioFilePath { get; set; }

    /// <summary>
    /// The start time of the simulation. This is the latest of the
    /// user specified time and the suitable start time from the Exchange data.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The starting cash.
    /// </summary>
    public decimal StartingCash { get; set; }

    /// <summary>
    /// The default bank account name to use.
    /// </summary>
    public TwoName DefaultBankAccName { get; set; } = new TwoName("Cash", "Portfolio");
}
