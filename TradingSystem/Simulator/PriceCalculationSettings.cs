using System;

namespace TradingSystem.Simulator
{
    /// <summary>
    /// Settings for the BuySell system.
    /// These are inherent settings for how the system works.
    /// </summary>
    public sealed class PriceCalculationSettings
    {
        /// <summary>
        /// Contains a random number generator for required points.
        /// </summary>
        public Random RandomNumbers
        {
            get;
        } = new Random(12345);

        /// <summary>
        /// The probability that a stock will have gone up from the opening price.
        /// </summary>
        public double UpTickProbability
        {
            get;
            set;
        }

        /// <summary>
        /// The relative size that a stock will have increased from the opening price.
        /// </summary>
        public double UpTickSize
        {
            get;
            set;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public PriceCalculationSettings(
            double upTickProbability = 0.5,
            double upTickSize = 0.01)
        {
            UpTickProbability = upTickProbability;
            UpTickSize = upTickSize;
        }
    }
}
