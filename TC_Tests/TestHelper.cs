using FinancialStructures.ReportLogging;

namespace TC_Tests
{
    public static class TestHelper
    {
        public static LogReporter ReportLogger = new LogReporter((critical, type, location, message) => NothingFunction(critical, type, location, message));

        private static void NothingFunction(string one, string two, string three, string four)
        {
        }
    }
}
