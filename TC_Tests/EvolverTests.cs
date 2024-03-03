using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Execution;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Common.Trading;

using NUnit.Framework;

using TradingSystem.MarketEvolvers;

using DecisionSystemFactory = Effanville.TradingStructures.Strategies.Decision.DecisionSystemFactory;

namespace TradingConsole.Tests;

internal class EventEvolverTests
{
    public static IEnumerable<TestCaseData> NewEvolverTestData()
    {
        string tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2015-01-21T08:00:00|2015-01-21T08:00:00|Barclays|Buy|21|
|2015-01-21T08:00:00|2015-01-21T08:00:00|stuff-Dunelm|Buy|4|
|2015-01-22T08:00:00|2015-01-22T08:00:00|Barclays|Buy|12|
|2015-01-22T08:00:00|2015-01-22T08:00:00|stuff-Dunelm|Buy|2|
|2015-01-23T08:00:00|2015-01-23T08:00:00|Barclays|Buy|7|
|2015-01-23T08:00:00|2015-01-23T08:00:00|stuff-Dunelm|Buy|1|";
        Dictionary<DateTime, TradeCollection> trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
        yield return new TestCaseData(
            DateTime.SpecifyKind(new DateTime(2015, 1, 20), DateTimeKind.Utc),
            new DateTime(2015, 1, 25),
            58,
            20366.116277008056754m,
            6,
            6,
            0,
            trades).SetName("TwoDayEvolutionTest");        
        tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2015-02-03T08:00:00|2015-02-03T08:00:00|-Barclays|Buy|21|
|2015-02-03T08:00:00|2015-02-03T08:00:00|stuff-Dunelm|Buy|4|
|2015-02-04T08:00:00|2015-02-04T08:00:00|-Barclays|Buy|11|
|2015-02-04T08:00:00|2015-02-04T08:00:00|stuff-Dunelm|Buy|2|
|2015-02-05T08:00:00|2015-02-05T08:00:00|-Barclays|Buy|7|
|2015-02-05T08:00:00|2015-02-05T08:00:00|stuff-Dunelm|Buy|1|
|2015-02-06T08:00:00|2015-02-06T08:00:00|-Barclays|Buy|4|
|2015-02-09T08:00:00|2015-02-09T08:00:00|-Barclays|Buy|3|
|2015-02-10T08:00:00|2015-02-10T08:00:00|-Barclays|Buy|2|
|2015-02-11T08:00:00|2015-02-11T08:00:00|-Barclays|Buy|2|
|2015-02-12T08:00:00|2015-02-12T08:00:00|-Barclays|Buy|1|
|2015-02-13T08:00:00|2015-02-13T08:00:00|-Barclays|Buy|1|
|2015-02-16T08:00:00|2015-02-16T08:00:00|-Barclays|Buy|1|";
        trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
        yield return new TestCaseData(
            DateTime.SpecifyKind(new DateTime(2015, 2, 1), DateTimeKind.Utc),
            new DateTime(2015, 3, 1),
            257,
            20825.4242126464855912m,
            13,
            13,
            0,
            trades).SetName("OneMonthEvolutionTest");
    }
    
    [TestCaseSource(nameof(NewEvolverTestData))]
    public void TestNewEvolver(
        DateTime startTime, 
        DateTime endTime, 
        int numberReports, 
        decimal expectedEndValue,
        int expectedNumberTrades,
        int expectedBuyTrades,
        int expectedSellTrades,
        Dictionary<DateTime, TradeCollection> expectedTrades)
    {
        var fileSystem = new MockFileSystem();
        string configureFile =
            File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, "example-database.xml"));
        string testFilePath = "c:/temp/exampleFile.xml";
        fileSystem.AddFile(testFilePath, configureFile);

        var logger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
        var stockExchange = StockExchangeFactory.Create(testFilePath, fileSystem, logger);
        foreach (var stock in stockExchange.Stocks)
        {
            foreach (var value in stock.Valuations)
            {
                value.Start = DateTime.SpecifyKind(value.Start, DateTimeKind.Utc);
            }
        }
        var settings = new EvolverSettings(startTime, endTime, TimeSpan.FromMinutes(1));
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

        if (!Directory.Exists("logs"))
        {
            Directory.CreateDirectory("logs");
        }

        logger.WriteReportsToFile($"logs\\{DateTime.Now:yyyy-MM-ddTHHmmss}{TestContext.CurrentContext.Test.Name}.log");
        var reports = logger.Reports;
        Assert.That(reports, Is.Not.Null);

        var actualTrades = evolver.Result.Trades;
        string mdTable = actualTrades.ConvertToTable();
        decimal finalValue = evolver.Result.Portfolio.TotalValue(Totals.All);
        Assert.Multiple(() =>
        {
            Assert.That(reports.Count(), Is.EqualTo(numberReports));
            Assert.That(finalValue, Is.EqualTo(expectedEndValue));
            Assert.That(actualTrades.TotalTrades, Is.EqualTo(expectedNumberTrades));
            Assert.That(actualTrades.TotalBuyTrades, Is.EqualTo(expectedBuyTrades));
            Assert.That(actualTrades.TotalSellTrades, Is.EqualTo(expectedSellTrades));
            if (expectedTrades.Count > 0)
            {
                CollectionAssert.AreEquivalent(expectedTrades, actualTrades.DailyTrades);
            }
        });
    }
}