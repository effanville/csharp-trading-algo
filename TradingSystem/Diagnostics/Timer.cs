using System;
using System.Diagnostics;

using Effanville.Common.Structure.Reporting;

namespace TradingSystem.Diagnostics
{
    /// <summary>
    /// Enables timing of operations. Timing starts upon creation and
    /// is recorded and logged upon disposal. Should be used in a using statement.
    /// </summary>
    public sealed class Timer : IDisposable
    {
        private readonly Stopwatch fWatch;
        private readonly IReportLogger fLogger;
        private readonly string fOperation;
        public Timer(IReportLogger logger, string operation)
        {
            fWatch = new Stopwatch();
            fLogger = logger;
            fOperation = operation;
            fWatch.Start();
        }

        /// <summary>
        /// Logs the time since creation of the timer. 
        /// <para/>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            fWatch.Stop();
            _ = fLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"{fOperation}: Time took {TimeSpanFriendlyString(fWatch.Elapsed)}");
        }

        private static string TimeSpanFriendlyString(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMilliseconds < 1000)
            {
                return $"{timeSpan.TotalMilliseconds}ms";
            }

            if (timeSpan.TotalSeconds < 60)
            {
                return $"{timeSpan.TotalSeconds}s";
            }

            if (timeSpan.TotalMinutes < 60)
            {
                return $"{timeSpan.Minutes}:{timeSpan.Seconds}";
            }

            return $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
        }
    }
}