namespace Effanville.TradingStructures.Strategies.Portfolio;

/// <summary>
/// Contains options for
/// </summary>
public sealed class PortfolioConstructionSettings
{
    public const string OptionsName = nameof(PortfolioConstructionSettings);

    /// <summary>
    /// The fraction of available cash to invest in any one decision.
    /// </summary>
    public decimal FractionInvest { get; set; } = 0.25m;
}
