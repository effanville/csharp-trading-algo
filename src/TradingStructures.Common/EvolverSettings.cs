using System;

namespace Effanville.TradingStructures.Common
{
    /// <summary>
    /// Settings required for a simulator to simulate.
    /// </summary>
    /// <remarks>
    /// Construct an instance.
    /// </remarks>
    public class EvolverSettings(string stockFilePath, DateTime startTime, DateTime endTime, TimeSpan evolutionIncrement)
    {
        public string StockFilePath { get; set; } = stockFilePath;

        /// <summary>
        /// The start time of the simulation. This is the latest of the
        /// user specified time and the suitable start time from the Exchange data.
        /// </summary>
        public DateTime StartTime
        {
            get;
            protected set;
        } = startTime;

        /// <summary>
        /// The end time of the simulation. This is the earliest of the
        /// user specified time and the latest time in the
        /// Exchange.
        /// </summary>
        public DateTime EndTime
        {
            get;
            protected set;
        } = endTime;

        /// <summary>
        /// The increment between times to trade at.
        /// </summary>
        public TimeSpan EvolutionIncrement
        {
            get;
            private set;
        } = evolutionIncrement.Seconds != 0 ? evolutionIncrement : new TimeSpan(1, 0, 0, 0);
    }
}
