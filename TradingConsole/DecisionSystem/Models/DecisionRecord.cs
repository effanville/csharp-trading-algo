using System;
using System.Collections.Generic;

namespace TradingConsole.DecisionSystem.Models
{
    /// <summary>
    /// Stores the record of all decisions made.
    /// </summary>
    public sealed class DecisionRecord
    {
        public Dictionary<DateTime, DecisionStatus> DailyDecisions
        {
            get;
        }

        public DecisionRecord()
        {
            DailyDecisions = new Dictionary<DateTime, DecisionStatus>();
        }

        public void AddForTheRecord(DateTime day, DecisionStatus status)
        {
            DailyDecisions.Add(day, status);
        }
    }
}
