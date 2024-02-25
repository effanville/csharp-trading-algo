using System;

using Effanville.FinancialStructures.Stocks;

using Nager.Date;

namespace TradingSystem.MarketEvolvers
{
    /// <summary>
    /// Settings required for a simulator to simulate.
    /// </summary>
    public sealed class TimeIncrementEvolverSettings : EvolverSettings
    {
        public DateTime BurnInEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// The code for the country to determine trading days.
        /// </summary>
        public CountryCode CountryDateCode
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

        public TimeIncrementEvolverSettings(DateTime startTime, DateTime endTime, TimeSpan evolutionIncrement, IStockExchange exchange, CountryCode countryCode = CountryCode.GB)
            : base(startTime, endTime, evolutionIncrement)
        {
            CountryDateCode = countryCode;
            Exchange = exchange;
            EnsureStartDatesConsistent();
        }

        public void DoesntRequireBurnIn() => BurnInEnd = StartTime;

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
