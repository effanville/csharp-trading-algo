using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

using Effanville.Common.Console;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;

using NUnit.Framework;

namespace TradingConsole.Tests
{
    [TestFixture]
    internal sealed class EndToEndTests
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
            void writeLine(string text) => _ = fConsoleOutput.AppendLine(text);
            void writeError(string text) => _ = fConsoleOutput.AppendLine(text);
            fConsole = new ConsoleInstance(writeError, writeLine);

            // Create the logger.
            var reports = new ErrorReports();
            void reportAction(ReportSeverity severity, ReportType reportType, string location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
                fConsole.WriteLine($"({reportType}) {text}");
            }

            fLogger = new LogReporter(reportAction, new SingleTaskQueue(), saveInternally: true);
        }

        [TearDown]
        public void TearDown()
        {
            fFileSystem = null;
            fConsoleOutput = null;
            fConsole = null;
            fLogger = null;
        }

        [TestCase("example-configure-file.csv")]
        [TestCase("small-exchange.csv")]
        public void Configure(string fileName)
        {
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, fileName));
            string testFilePath = "c:/temp/exampleFile.csv";
            fFileSystem.AddFile(testFilePath, configureFile);
            string[] args = new[] { "configure", "--stockFilePath", testFilePath };
            _ = Program.InternalMain(args, fFileSystem, fConsole, fLogger);

            Assert.IsTrue(fFileSystem.File.Exists("c:/temp/exampleFile.xml"));
            Assert.AreEqual(2, fLogger.Reports.Count());
            string expectedOutput = $"(Information) Configured StockExchange from file c:/temp/exampleFile.csv.{Environment.NewLine}(Information) Saved StockExchange at c:/temp/exampleFile.xml{Environment.NewLine}";
            Assert.AreEqual(expectedOutput, fConsoleOutput.ToString());
        }

        [TestCase("example-database-empty.xml")]
        public void Download(string fileName)
        {
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, fileName));
            string testFilePath = "c:\\temp\\exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);
            string[] args = new[] { "download", "all", "--stockFilePath", testFilePath, "--start", "1/1/2010", "--end", "1/1/2023" };
            _ = Program.InternalMain(args, fFileSystem, fConsole, fLogger);
            Assert.GreaterOrEqual(fLogger.Reports.Count(), 2);
        }

        [Test]
        public void BasicRun()
        {
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, "example-database.xml"));
            string testFilePath = "c:/temp/exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);

            string[] args = new[] { "simulate", "--stockFilePath", testFilePath, "--start", "2015-01-05T08:00:00", "--end", "2019-12-12T08:00:00", "--startCash", "20000", "--decision", "BuyAll", "--invFrac", "0.25" };
            _ = Program.InternalMain(args, fFileSystem, fConsole, fLogger);
            Assert.GreaterOrEqual(fLogger.Reports.Count(), 65);
        }

        [Test]
        public void FiveDayStatsRun()
        {
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, "example-database.xml"));
            string testFilePath = "c:/temp/exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);

            string[] args = new[] { "simulate", "--stockFilePath", testFilePath, "--start", "2015-01-05T08:00+00:00", "--end", "2019-12-12T08:00:00", "--startCash", "20000", "--invFrac", "1" };
            _ = Program.InternalMain(args, fFileSystem, fConsole, fLogger);
            Assert.GreaterOrEqual(fLogger.Reports.Count(), 80);
        }
    }
}
