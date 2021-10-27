using System;

using FinancialStructures.StockStructures;

namespace TradingSystem.Simulator
{
    /// <summary>
    /// Settings required for a simulator to simulate.
    /// </summary>
    public sealed class SimulatorSettings
    {
        /// <summary>
        /// The start time of the simulation. This is the latest of the
        /// user specified time and the suitable start time from the Exchange data.
        /// </summary>
        public DateTime StartTime
        {
            get;
            private set;
        }

        public DateTime BurnInEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// The end time of the simulation. This is the earliest of the
        /// user specified time and the latest time in the
        /// Exchange.
        /// </summary>
        public DateTime EndTime
        {
            get;
            private set;
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
        /// The stock exchange to use for this simulation.
        /// </summary>
        public IStockExchange Exchange
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SimulatorSettings(DateTime startTime, DateTime endTime, TimeSpan evolutionIncrement, IStockExchange exchange)
        {
            StartTime = startTime;
            EndTime = endTime;
            EvolutionIncrement = evolutionIncrement.Seconds != 0 ? evolutionIncrement : new TimeSpan(1, 0, 0, 0);


            Exchange = exchange;
            EnsureStartDatesConsistent();
        }

        public void DoesntRequireBurnIn()
        {
            BurnInEnd = StartTime;
        }

        /// <summary>
        /// Ensures that the start and end times can be used based on the exchange provided.
        /// </summary>
        private void EnsureStartDatesConsistent()
        {
            var earliest = Exchange.LatestEarliestDate();
            var latest = Exchange.LastDate();
            if (StartTime < earliest)
            {
                StartTime = earliest;
            }
            if (EndTime > latest)
            {
                EndTime = latest;
            }

            BurnInEnd = StartTime + EvolutionIncrement * (long)((EndTime - StartTime) / (2 * EvolutionIncrement));
        }
    }
}
