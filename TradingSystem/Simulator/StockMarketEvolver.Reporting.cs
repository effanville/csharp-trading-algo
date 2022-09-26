using System;

using Common.Structure.Reporting;

namespace TradingSystem.Simulator
{
    public static partial class StockMarketEvolver
    {
        /// <summary>
        /// Objects used for reporting information during the stock market evolution.
        /// </summary>
        public sealed class Reporting
        {
            /// <summary>
            /// The reporting method at the start of the simulation.
            /// </summary>
            public Action<string> StartReportCallback
            {
                get;
            }

            /// <summary>
            /// The reporting method during simulation.
            /// </summary>
            public Action<DateTime, string> ReportCallback
            {
                get;
            }

            /// <summary>
            /// The end of the evolution reporting method.
            /// </summary>
            public Action<string> EndReportCallback
            {
                get;
            }

            /// <summary>
            /// The logger to report errors.
            /// </summary>
            public IReportLogger Logger
            {
                get;
            }

            /// <summary>
            /// Construct an instance.
            /// </summary>
            public Reporting(Action<string> startReportCallback, Action<DateTime, string> reportCallback, Action<string> endReportCallback, IReportLogger logger)
            {
                StartReportCallback = startReportCallback;
                ReportCallback = reportCallback;
                EndReportCallback = endReportCallback;
                Logger = logger;
            }
        }
    }
}
