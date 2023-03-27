using Common.Structure.Reporting;

namespace TradingConsole.Tests
{
    internal static class TestHelper
    {
        public static LogReporter ReportLogger = new LogReporter((critical, type, location, message) => NothingFunction(critical, type, location, message));

        private static void NothingFunction(ReportSeverity one, ReportType two, string three, string four)
        {
        }
    }
}
