using System;

using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;

using Nager.Date;

namespace Effanville.TradingSystem.MarketEvolvers
{
    /// <summary>
    /// Settings required for a simulator to simulate.
    /// </summary>
    public sealed class TimeIncrementEvolverSettings : EvolverSettings
    {
        public DateTime BurnInStart
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
        }

        public TimeIncrementEvolverSettings(DateTime startTime, DateTime endTime, TimeSpan evolutionIncrement, IStockExchange exchange, CountryCode countryCode = CountryCode.GB)
            : base(startTime, endTime, evolutionIncrement)
        {
            BurnInStart = startTime;
            CountryDateCode = countryCode;
            Exchange = exchange;
            EnsureStartDatesConsistent();
        }

        /// <summary>
        /// Ensures that the start and end times can be used based on the exchange provided.
        /// </summary>
        private void EnsureStartDatesConsistent()
        {
            var earliest = Exchange.LatestEarliestDate();
            var latest = Exchange.LastDate();
            if (BurnInStart < earliest)
            {
                BurnInStart = earliest;
            }
            if (EndTime > latest)
            {
                EndTime = latest;
            }

            StartTime = BurnInStart + EvolutionIncrement * (long)((EndTime - BurnInStart) / (2 * EvolutionIncrement));
        }        
        
        public void DoesntRequireBurnIn() => StartTime = BurnInStart;
    }
}
