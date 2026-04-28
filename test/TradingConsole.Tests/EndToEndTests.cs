using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Console;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Persistence;
using Effanville.TradingConsole.Commands.ExchangeCreation;
using Effanville.TradingConsole.Commands.Execution;
using Effanville.TradingStructures.Common.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NSubstitute;

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

            var persistence = new ExchangePersistence(new LoggerFactory());
            ILogger<ConfigureCommand> logger = Substitute.For<ILogger<ConfigureCommand>>();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var statisticsCommand = new ConfigureCommand(
                mockFileSystem,
                logger,
                config,
                persistence,
                new StockExchangeFactory(
                    Substitute.For<ILogger<StockExchangeFactory>>(),
                    new LoggerFactory(),
                    mockFileSystem));
            bool isValidated = statisticsCommand.Validate();

            Assert.That(isValidated, Is.True);

            int executed = statisticsCommand.Execute();
            Assert.Multiple(() =>
            {
                Assert.That(executed, Is.EqualTo(0));
                Assert.That(mockFileSystem.File.Exists("c:/temp/exampleFile.xml"), Is.True);
                string file = mockFileSystem.File.ReadAllText("c:/temp/exampleFile.xml");
                Assert.That(file, Contains.Substring("<StockExchange "));
                Assert.That(file, Contains.Substring("<Stocks>"));
                Assert.That(file, Contains.Substring("<Company>Barclays</Company>"));
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

            var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            var persistence = new ExchangePersistence(new LoggerFactory());

            ILogger<DownloadAllCommand> logger = Substitute.For<ILogger<DownloadAllCommand>>();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var downloadAllCommand = new DownloadAllCommand(mockFileSystem, logger, reportLogger, config, persistence);

            bool isValidated = downloadAllCommand.Validate();

            Assert.That(isValidated, Is.True);

            int executed = downloadAllCommand.Execute();
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
            var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            ILogger<SimulationCommand> logger = Substitute.For<ILogger<SimulationCommand>>();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var simulationCommand = new SimulationCommand(mockFileSystem, new TimerFactory(new LoggerFactory()), logger, reportLogger, config);

            bool isValidated = simulationCommand.Validate();
            Assert.That(isValidated, Is.True);

            int executed = simulationCommand.Execute();
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
            var reportLogger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            ILogger<SimulationCommand> logger = Substitute.For<ILogger<SimulationCommand>>();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(new ConsoleCommandArgs(args).GetEffectiveArgs())
                .AddEnvironmentVariables()
                .Build();
            var simulationCommand = new SimulationCommand(mockFileSystem, new TimerFactory(new LoggerFactory()), logger, reportLogger, config);

            bool isValidated = simulationCommand.Validate();
            Assert.That(isValidated, Is.True);

            int executed = simulationCommand.Execute();
            Assert.Multiple(() =>
            {
                Assert.That(executed, Is.EqualTo(0));
                Assert.That(reportLogger.Reports.Count(), Is.GreaterThanOrEqualTo(65));
            });
        }
    }
}
