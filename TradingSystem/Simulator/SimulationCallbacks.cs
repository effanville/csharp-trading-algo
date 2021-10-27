using System;

namespace TradingSystem.Simulator
{
    public sealed class SimulationCallbacks
    {
        public Action<string> StartReportCallback
        {
            get;
        }

        public Action<DateTime, string> ReportCallback
        {
            get;
        }

        public Action<string> EndReportCallback
        {
            get;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public SimulationCallbacks(Action<string> startReportCallback, Action<DateTime, string> reportCallback, Action<string> endReportCallback)
        {
            StartReportCallback = startReportCallback;
            ReportCallback = reportCallback;
            EndReportCallback = endReportCallback;
        }
    }
}
