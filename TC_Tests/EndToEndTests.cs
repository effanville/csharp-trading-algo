using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

using Common.Console;
using Common.Structure.Reporting;

using NUnit.Framework;

namespace TradingConsole.Tests
{
    [TestFixture]
    public class EndToEndTests
    {
        private MockFileSystem fFileSystem;
        private StringBuilder fConsoleOutput;
        private IConsole fConsole;
        private IReportLogger fLogger;

        [SetUp]
        public void Setup()
        {
            fFileSystem = new MockFileSystem();
            // Create the Console to write output.
            fConsoleOutput = new StringBuilder();
            void writeLine(string text) => fConsoleOutput.AppendLine(text);
            void writeError(string text) => fConsoleOutput.AppendLine(text);
            fConsole = new ConsoleInstance(writeError, writeLine);

            // Create the logger.
            var reports = new ErrorReports();
            void reportAction(ReportSeverity severity, ReportType reportType, ReportLocation location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
                fConsole.WriteLine($"({reportType}) {text}");
            }
            fLogger = new LogReporter(reportAction);
        }

        [TearDown]
        public void TearDown()
        {
            fFileSystem = null;
            fConsoleOutput = null;
            fConsole = null;
            fLogger = null;
        }

        [Test]
        public void Configure()
        {
            var configureFile = File.ReadAllText($"{TestConstants.ExampleFilesLocation}\\example-configure-file.csv");
            string testFilePath = "c:/temp/exampleFile.csv";
            fFileSystem.AddFile(testFilePath, configureFile);
            string[] args = new[] { "configure", "--stockFilePath", testFilePath };
            Program.InternalMain(args, fFileSystem, fConsole, fLogger);

            Assert.IsTrue(fFileSystem.File.Exists("c:/temp/exampleFile.xml"));
            Assert.AreEqual(2, fLogger.Reports.Count());
            string expectedOutput = "(Information) Configured StockExchange from file c:/temp/exampleFile.csv.\r\n(Information) Saved StockExchange at c:/temp/exampleFile.xml\r\n";
            Assert.AreEqual(expectedOutput, fConsoleOutput.ToString());
        }

        [Test]
        public void Download()
        {
            var configureFile = File.ReadAllText($"{TestConstants.ExampleFilesLocation}\\example-database-empty.xml");
            string testFilePath = "c:\\temp\\exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);
            string[] args = new[] { "download", "all", "--stockFilePath", testFilePath, "--start", "1/1/2015", "--end", "1/1/2020" };
            Program.InternalMain(args, fFileSystem, fConsole, fLogger);
            Assert.AreEqual(4, fLogger.Reports.Count());
        }

        [Test]
        public void BasicRun()
        {
            var configureFile = File.ReadAllText($"{TestConstants.ExampleFilesLocation}\\example-database.xml");
            string testFilePath = "c:\\temp\\exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);

            string[] args = new[] { "simulate", "--stockFilePath", testFilePath, "--start", "2015-01-05T08:00:00", "--end", "2019-12-12T08:00:00", "--startCash", "20000", "--decision", "BuyAll", "--invFrac", "0.25" };
            Program.InternalMain(args, fFileSystem, fConsole, fLogger);
            Assert.AreEqual(105, fLogger.Reports.Count());
            string expectedOutput = fConsoleOutput.ToString();
        }

        [Test]
        public void FiveDayStatsRun()
        {
            var configureFile = File.ReadAllText($"{TestConstants.ExampleFilesLocation}\\example-database.xml");
            string testFilePath = "c:/temp/exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);

            string[] args = new[] { "simulate", "--stockFilePath", testFilePath, "--start", "5/1/2015", "--end", "12/12/2019", "--startCash", "20000", "--invFrac", "1" };
            Program.InternalMain(args, fFileSystem, fConsole, fLogger);
            Assert.AreEqual(43, fLogger.Reports.Count());
            string expectedOutput = fConsoleOutput.ToString();
        }
    }
}
