using System;
using System.Collections.Generic;

namespace TradingSystem.Simulator.Trading.Decisions
{
    /// <summary>
    /// Stores the record of all decisions made.
    /// </summary>
    public sealed class DecisionHistory
    {
        public Dictionary<DateTime, DecisionStatus> DailyDecisions
        {
            get;
        }

        public DecisionHistory()
        {
            DailyDecisions = new Dictionary<DateTime, DecisionStatus>();
        }

        public void AddForTheRecord(DateTime day, DecisionStatus status)
        {
            DailyDecisions.Add(day, status);
        }
    }
}
