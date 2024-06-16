using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

using Effanville.Common.Console;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.TradingConsole.Commands.ExchangeCreation;
using Effanville.TradingConsole.Commands.Execution;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using TradingConsole.Tests;

namespace Effanville.TradingConsole.Tests
{
    [TestFixture]
    internal sealed class EndToEndTests
    {
        [TestCase("example-configure-file.csv")]
        [TestCase("small-exchange.csv")]
        public void Configure(string fileName)
        {
            var mockFileSystem = new MockFileSystem();
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, fileName));
            string testFilePath = "c:/temp/exampleFile.csv";
            mockFileSystem.AddFile(testFilePath, configureFile);
            string[] args = new[] { "configure", "--stockFilePath", testFilePath };
            
            var consoleInstance = new ConsoleInstance(null, null);
            var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            var mock = new Mock<ILogger<ConfigureCommand>>();
            ILogger<ConfigureCommand> logger = mock.Object;
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var statisticsCommand = new ConfigureCommand(mockFileSystem, logger, reportLogger);
            bool isValidated = statisticsCommand.Validate(consoleInstance, config);
            
            Assert.That(isValidated, Is.True);

            int executed = statisticsCommand.Execute(consoleInstance, config);
            Assert.Multiple(() =>
            {
                Assert.That(executed, Is.EqualTo(0));
                Assert.That(mockFileSystem.File.Exists("c:/temp/exampleFile.xml"), Is.True);
                var reports = reportLogger.Reports;
                Assert.That(reports.Count(), Is.EqualTo(2));
                Assert.That(reports[0].Message, Is.EqualTo("Configured StockExchange from file c:/temp/exampleFile.csv."));
                Assert.That(reports[1].Message, Is.EqualTo("Saved StockExchange at c:/temp/exampleFile.xml"));
            });
        }

        [TestCase("example-database-empty.xml")]
        public void Download(string fileName)
        {
            var mockFileSystem = new MockFileSystem();
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, fileName));
            string testFilePath = "c:\\temp\\exampleFile.xml";
            mockFileSystem.AddFile(testFilePath, configureFile);
            string[] args = new[] { "download", "all", "--stockFilePath", testFilePath, "--start", "1/1/2010", "--end", "1/1/2023" };
            
            var consoleInstance = new ConsoleInstance(null, null);
            var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            var mock = new Mock<ILogger<DownloadAllCommand>>();
            ILogger<DownloadAllCommand> logger = mock.Object;
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var downloadAllCommand = new DownloadAllCommand(mockFileSystem, logger, reportLogger);
            
            bool isValidated = downloadAllCommand.Validate(consoleInstance, config);
            
            Assert.That(isValidated, Is.True);

            int executed = downloadAllCommand.Execute(consoleInstance, config);
            Assert.Multiple(() =>
            {
                Assert.That(executed, Is.EqualTo(0));
                Assert.That(reportLogger.Reports.Count(), Is.GreaterThanOrEqualTo(2));
            });
        }

        [Test]
        public void BasicRun()
        {
            var mockFileSystem = new MockFileSystem();
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, "example-database.xml"));
            string testFilePath = "c:/temp/exampleFile.xml";
            mockFileSystem.AddFile(testFilePath, configureFile);

            string[] args = new[] { "simulate", "--stockFilePath", testFilePath, "--start", "2015-01-05T08:00:00", "--end", "2019-12-12T08:00:00", "--startCash", "20000", "--decision", "BuyAll", "--invFrac", "0.25" };
            var consoleInstance = new ConsoleInstance(null, null);
            var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            var mock = new Mock<ILogger<SimulationCommand>>();
            ILogger<SimulationCommand> logger = mock.Object;
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var simulationCommand = new SimulationCommand(mockFileSystem, logger, reportLogger);
            
            bool isValidated = simulationCommand.Validate(consoleInstance, config);
            Assert.That(isValidated, Is.True);

            int executed = simulationCommand.Execute(consoleInstance, config);
            Assert.Multiple(() =>
            {
                Assert.That(executed, Is.EqualTo(0));
                Assert.That(reportLogger.Reports.Count(), Is.GreaterThanOrEqualTo(65));
            });
        }

        [Test]
        public void FiveDayStatsRun()
        {
            var mockFileSystem = new MockFileSystem();
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, "example-database.xml"));
            string testFilePath = "c:/temp/exampleFile.xml";
            mockFileSystem.AddFile(testFilePath, configureFile);

            string[] args = new[] { "simulate", "--stockFilePath", testFilePath, "--start", "2015-01-05T08:00+00:00", "--end", "2019-12-12T08:00:00", "--startCash", "20000", "--invFrac", "1" };
            var consoleInstance = new ConsoleInstance(null, null);
            var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            var mock = new Mock<ILogger<SimulationCommand>>();
            ILogger<SimulationCommand> logger = mock.Object;
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var simulationCommand = new SimulationCommand(mockFileSystem, logger, reportLogger);
            
            bool isValidated = simulationCommand.Validate(consoleInstance, config);
            Assert.That(isValidated, Is.True);
            
            int executed = simulationCommand.Execute(consoleInstance, config);
            Assert.Multiple(() =>
            {
                Assert.That(executed, Is.EqualTo(0));
                Assert.That(reportLogger.Reports.Count(), Is.GreaterThanOrEqualTo(65));
            });
        }
    }
}
