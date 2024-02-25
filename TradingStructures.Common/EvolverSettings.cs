namespace Effanville.TradingStructures.Common
{
    /// <summary>
    /// Settings required for a simulator to simulate.
    /// </summary>
    public class EvolverSettings
    {
        /// <summary>
        /// The start time of the simulation. This is the latest of the
        /// user specified time and the suitable start time from the Exchange data.
        /// </summary>
        public DateTime StartTime
        {
            get;
            protected set;
        }

        /// <summary>
        /// The end time of the simulation. This is the earliest of the
        /// user specified time and the latest time in the
        /// Exchange.
        /// </summary>
        public DateTime EndTime
        {
            get;
            protected set;
        }

        /// <summary>
        /// The increment between times to trade at.
        /// </summary>
        public TimeSpan EvolutionIncrement
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public EvolverSettings(DateTime startTime, DateTime endTime, TimeSpan evolutionIncrement)
        {
            StartTime = startTime;
            EndTime = endTime;
            EvolutionIncrement = evolutionIncrement.Seconds != 0 ? evolutionIncrement : new TimeSpan(1, 0, 0, 0);
        }
    }
}
