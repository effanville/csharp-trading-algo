using System;

namespace Effanville.TradingStructures.MarketData;

/// <summary>
/// Settings for the BuySell system.
/// These are inherent settings for how the system works.
/// </summary>
public sealed class PriceCalculationSettings
{
    public const string OptionsName = nameof(PriceCalculationSettings);
    public PriceType PriceType { get; set; } = PriceType.RandomWobble;

    /// <summary>
    /// Contains a random number generator for required points.
    /// </summary>
    public Random RandomNumbers { get; } = new Random(12345);

    /// <summary>
    /// The probability that a stock will have gone up from the opening price.
    /// </summary>
    public double UpTickProbability { get; set; } = 0.5d;

    /// <summary>
    /// The relative size that a stock will have increased from the opening price.
    /// </summary>
    public double UpTickSize { get; set; } = 0.01;
}
