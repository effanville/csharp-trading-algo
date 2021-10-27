using System;
using System.Diagnostics;
using Common.Structure.Reporting;

namespace TradingSystem
{
    public sealed class Timer : IDisposable
    {
        private Stopwatch fWatch;
        private IReportLogger fLogger;
        private readonly string fOperation;
        public Timer(IReportLogger logger, string operation)
        {
            fWatch = new Stopwatch();
            fLogger = logger;
            fOperation = operation;
            fWatch.Start();
        }

        public void Dispose()
        {
            fWatch.Stop();
            _ = fLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"{fOperation} took {TimeSpanFriendlyString(fWatch.Elapsed)}");
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
                return $"{timeSpan.TotalMinutes}:{timeSpan.TotalSeconds}";
            }

            return $"{timeSpan.TotalHours}:{timeSpan.TotalMinutes}:{timeSpan.TotalSeconds}";
        }
    }
}