using System;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;

namespace TradingConsole.Simulation
{
    /// <summary>
    /// Contains all parameters for a simulation.
    /// </summary>
    public class SimulationParameters
    {
        /// <summary>
        /// Contains a random number generator for required points.
        /// </summary>
        public Random RandomNumbers
        {
            get;
        } = new Random();

        /// <summary>
        /// The start time of the simulation. This is the latest of the 
        /// user specified time and the suitable start time from the Exchange data.
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
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
        }

        /// <summary>
        /// The probability that a stock will have gone up from the opening price.
        /// </summary>
        public double UpTickProbability
        {
            get;
        } = 0.5;

        /// <summary>
        /// The relative size that a stock will have increased from the opening price.
        /// </summary>
        public double UpTickSize
        {
            get;
        } = 0.05;

        /// <summary>
        /// The fixed cost associated with each trade.
        /// </summary>
        public double TradeCost
        {
            get;
        } = 6;

        /// <summary>
        /// The starting cash.
        /// </summary>
        public double StartingCash
        {
            get;
        }

        /// <summary>
        /// The default bank account name to use.
        /// </summary>
        public NameData BankAccData
        {
            get;
        } = new NameData("Cash", "Portfolio");

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SimulationParameters(DateTime startDate, DateTime endDate, TimeSpan tradingGap, double startingCash)
        {
            StartTime = startDate;
            EndTime = endDate == default(DateTime) ? DateTime.Today : endDate;
            EvolutionIncrement = tradingGap.Seconds != 0 ? tradingGap : new TimeSpan(1, 0, 0, 0);
            StartingCash = startingCash;
        }

        /// <summary>
        /// Ensures that the start and end times can be used based on the exchange provided.
        /// </summary>
        /// <param name="exchange"></param>
        public void EnsureStartEndConsistent(IStockExchange exchange)
        {
            var earliest = exchange.LatestEarliestDate();
            var latest = exchange.LastDate();
            if (StartTime < earliest)
            {
                StartTime = earliest;
            }
            if (EndTime > latest)
            {
                EndTime = latest;
            }
        }
    }
}
