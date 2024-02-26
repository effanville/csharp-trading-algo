﻿using System;
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

using NUnit.Framework;

using TradingSystem.Decisions;
using TradingSystem.ExecutionStrategies;
using TradingSystem.MarketEvolvers;
using TradingSystem.PortfolioStrategies;
using TradingSystem.Trading;

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
            60,
            20366.116277008056754m,
            6,
            6,
            0,
            trades);
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
                //CollectionAssert.AreEquivalent(expectedTrades, actualTrades.DailyTrades);
            }
        });
    }
}