using System;

namespace Effanville.TradingStructures.MarketData;

/// <summary>
/// Settings for the BuySell system.
/// These are inherent settings for how the system works.
/// </summary>
/// <remarks>
/// Construct an instance.
/// </remarks>
public sealed class PriceCalculationSettings(
    PriceType priceType,
    double upTickProbability,
    double upTickSize)
{
    public PriceType PriceType { get; } = priceType;

    /// <summary>
    /// Contains a random number generator for required points.
    /// </summary>
    public Random RandomNumbers { get; } = new Random(12345);

    /// <summary>
    /// The probability that a stock will have gone up from the opening price.
    /// </summary>
    public double UpTickProbability { get; } = upTickProbability;

    /// <summary>
    /// The relative size that a stock will have increased from the opening price.
    /// </summary>
    public double UpTickSize { get; } = upTickSize;

    public static PriceCalculationSettings Default() => new(PriceType.RandomWobble, 0.5, 0.01);
}
