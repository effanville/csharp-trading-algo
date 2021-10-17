using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Common.Console;
using Common.Structure.Reporting;
using NUnit.Framework;
using TradingConsole;

namespace TC_Tests
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
            Program.Main(args, fFileSystem, fConsole, fLogger);

            Assert.IsTrue(fFileSystem.File.Exists("c:/temp/exampleFile.xml"));
            Assert.AreEqual(2, fLogger.Reports.Count());
            string expectedOutput = "(Information) Configured StockExchange from file c:/temp/exampleFile.csv.\r\n(Information) Saved StockExchange at c:/temp/exampleFile.xml\r\n";
            Assert.AreEqual(expectedOutput, fConsoleOutput.ToString());
        }

        [Test]
        public void Download()
        {
            var configureFile = File.ReadAllText($"{TestConstants.ExampleFilesLocation}\\example-database-empty.xml");
            string testFilePath = "c:/temp/exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);
            string[] args = new[] { "download", "all", "--stockFilePath", testFilePath, "--start", "1/1/2015", "--end", "1/1/2020" };
            Program.Main(args, fFileSystem, fConsole, fLogger);
            Assert.AreEqual(0, fLogger.Reports.Count());
            Assert.AreEqual("", fConsoleOutput.ToString());
        }

        [Test]
        public void BasicRun()
        {
            var configureFile = File.ReadAllText($"{TestConstants.ExampleFilesLocation}\\example-database.xml");
            string testFilePath = "c:/temp/exampleFile.xml";
            fFileSystem.AddFile(testFilePath, configureFile);

            string[] args = new[] { "simulate", "--stockFilePath", testFilePath, "--start", "5/1/2015", "--end", "12/12/2019", "--startingCash", "20000", "--decision", "BuyAll" };
            Program.Main(args, fFileSystem, fConsole, fLogger);
            Assert.AreEqual(74, fLogger.Reports.Count());
            string expectedOutput = fConsoleOutput.ToString();
        }
    }
}
