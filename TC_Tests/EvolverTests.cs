using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Stocks;

using NUnit.Framework;

using TradingSystem.Decisions;

using TradingSystem.Diagnostics;
using TradingSystem.ExecutionStrategies;
using TradingSystem.MarketEvolvers;
using TradingSystem.PortfolioStrategies;

namespace TradingConsole.Tests;

internal class EventEvolverTests
{
    [Test]
    public void TestNewEvolver()
    {
        var fileSystem = new MockFileSystem();
        string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, "example-database.xml"));
        string testFilePath = "c:/temp/exampleFile.xml";
        fileSystem.AddFile(testFilePath, configureFile);

        var logger = new LogReporter(null, saveInternally: true);
        var stockExchange = StockExchangeFactory.Create(testFilePath, fileSystem, logger);
        DateTime startTime = new DateTime(2015, 1, 20);
        var settings = new EvolverSettings(startTime, new DateTime(2015, 1, 25), TimeSpan.FromMinutes(1));
        var startSettings = new PortfolioStartSettings("", startTime, 20000m);
        var constructionSettings = PortfolioConstructionSettings.Default();
        var portfolioManager = PortfolioManager.LoadFromFile(fileSystem, startSettings, constructionSettings, logger);
        var evolver = new EventEvolver(
            settings,
            stockExchange,
            portfolioManager,
            StrategyType.TimeIncrementExecution,
            DecisionSystemFactory.Create(new DecisionSystemFactory.Settings(DecisionSystem.BuyAll)),
            logger);
        using (new Timer(logger, "Execution"))
        {
            evolver.Initialise();
            evolver.Start();
            while (evolver.IsActive)
            {
                _ = Task.Delay(100);
            }
        }

        var reports = logger.Reports;
        Assert.IsNotNull(reports);
        Assert.AreEqual(56, reports.Count());
    }
}