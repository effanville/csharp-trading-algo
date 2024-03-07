using System;
using System.Diagnostics;

using Effanville.Common.Structure.Reporting;

namespace Effanville.TradingStructures.Common.Diagnostics
{
    /// <summary>
    /// Enables timing of operations. Timing starts upon creation and
    /// is recorded and logged upon disposal. Should be used in a using statement.
    /// </summary>
    public sealed class Timer : IDisposable
    {
        private readonly Stopwatch _watch;
        private readonly IReportLogger _logger;
        private readonly string _operation;
        public Timer(IReportLogger logger, string operation)
        {
            _watch = new Stopwatch();
            _logger = logger;
            _operation = operation;
            _watch.Start();
        }

        /// <summary>
        /// Logs the time since creation of the timer. 
        /// <para/>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            _watch.Stop();
            _ = _logger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"{_operation}: Time took {TimeSpanFriendlyString(_watch.Elapsed)}");
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
                return $"{timeSpan.Minutes}:{timeSpan.Seconds:F2}";
            }

            return $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
        }
    }
}