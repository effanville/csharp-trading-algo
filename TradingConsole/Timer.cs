using System;
using System.Diagnostics;
using Common.Structure.Reporting;

namespace TradingConsole
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
            _ = fLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Execution, $"{fOperation} took {fWatch.Elapsed}");
        }
    }
}