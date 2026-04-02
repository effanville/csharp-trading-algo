using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingStructures.Common.Trading;

using NUnit.Framework;

using TradingConsole.Tests;

using DecisionSystemFactory = Effanville.TradingStructures.Strategies.Decision.DecisionSystemFactory;
using Effanville.FinancialStructures.Stocks.Statistics;
using Microsoft.Extensions.Hosting;
using Effanville.TradingSystem.DependencyInjection;

namespace Effanville.TradingSystem.Tests.MarketEvolvers;

internal class EventEvolverTests
{
    public static IEnumerable<TestCaseData> NewEvolverTestData()
    {
        string tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2015-01-22T08:00:00|2015-01-22T08:00:00|-Barclays|Buy|21|
|2015-01-22T08:00:00|2015-01-22T08:00:00|stuff-Dunelm|Buy|4|
|2015-01-23T08:00:00|2015-01-23T08:00:00|-Barclays|Buy|11|
|2015-01-23T08:00:00|2015-01-23T08:00:00|stuff-Dunelm|Buy|2|
";
        Dictionary<DateTime, TradeCollection> trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
        yield return new TestCaseData(
            "example-database.xml",
            DecisionSystem.BuyAll,
            null, 1, 1.05, 1.0,
            DateTime.SpecifyKind(new DateTime(2015, 1, 20), DateTimeKind.Utc),
            new DateTime(2015, 1, 25),
            33,
            20072.982838592529318m,
            4,
            4,
            0,
            trades).SetName("TwoDayEvolutionTest");
        tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2015-02-16T08:00:00|2015-02-16T08:00:00|-Barclays|Buy|19|
|2015-02-16T08:00:00|2015-02-16T08:00:00|stuff-Dunelm|Buy|4|
|2015-02-17T08:00:00|2015-02-17T08:00:00|-Barclays|Buy|11|
|2015-02-17T08:00:00|2015-02-17T08:00:00|stuff-Dunelm|Buy|2|
|2015-02-18T08:00:00|2015-02-18T08:00:00|-Barclays|Buy|6|
|2015-02-18T08:00:00|2015-02-18T08:00:00|stuff-Dunelm|Buy|1|
|2015-02-19T08:00:00|2015-02-19T08:00:00|-Barclays|Buy|4|
|2015-02-20T08:00:00|2015-02-20T08:00:00|-Barclays|Buy|3|
|2015-02-23T08:00:00|2015-02-23T08:00:00|-Barclays|Buy|2|
|2015-02-24T08:00:00|2015-02-24T08:00:00|-Barclays|Buy|1|
|2015-02-25T08:00:00|2015-02-25T08:00:00|-Barclays|Buy|1|
|2015-02-26T08:00:00|2015-02-26T08:00:00|-Barclays|Buy|1|
|2015-02-27T08:00:00|2015-02-27T08:00:00|-Barclays|Buy|1|
";
        trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
        yield return new TestCaseData(
            "example-database.xml",
            DecisionSystem.BuyAll,
            null, 1, 1.05, 1.0,
            DateTime.SpecifyKind(new DateTime(2015, 2, 1), DateTimeKind.Utc),
            new DateTime(2015, 3, 1),
            117,
            19810.9660935974140205m,
            13,
            13,
            0,
            trades).SetName("OneMonthEvolutionTest");
    }

    [TestCaseSource(nameof(NewEvolverTestData))]
    public async Task TestNewEvolver(
        string databaseName,
        DecisionSystem decisions,
        List<StockStatisticType> stockStatistics,
        int dayAfterPredictor,
        double buyThreshold,
        double sellThreshold,
        DateTime startTime,
        DateTime endTime,
        int numberReports,
        decimal expectedEndValue,
        int expectedNumberTrades,
        int expectedBuyTrades,
        int expectedSellTrades,
        Dictionary<DateTime, TradeCollection> expectedTrades)
    {
        var startSettings = new PortfolioStartSettings("", startTime, 20000m);
        var decisionParameters = new DecisionSystemFactory.Settings(decisions, stockStatistics, buyThreshold, sellThreshold, dayAfterPredictor);

        var fileSystem = new MockFileSystem();
        string configureFile =
            File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, databaseName));
        string testFilePath = "c:/temp/exampleFile.xml";
        fileSystem.AddFile(testFilePath, configureFile);

        var logger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);

        var builder = new HostApplicationBuilder();
        _ = builder.Logging.RegisterLogging(logger);
        _ = builder.Services.RegisterTradingServices(
            testFilePath,
            startTime,
            endTime,
            TimeSpan.FromMinutes(1),
            startSettings,
            PortfolioConstructionSettings.Default(),
            decisionParameters,
            fileSystem);
        var host = builder.Build();
        var result = await host.RunSystemAsync();

        if (!Directory.Exists("logs"))
        {
            _ = Directory.CreateDirectory("logs");
        }

        logger.WriteReportsToFile($"logs\\{DateTime.Now:yyyy-MM-ddTHHmmss}{TestContext.CurrentContext.Test.Name}.log");
        var reports = logger.Reports;
        Assert.That(reports, Is.Not.Null);

        var actualTrades = result.Trades;
        string mdTable = actualTrades.ConvertToTable();
        decimal finalValue = result.Portfolio.TotalValue(Totals.All);
        Assert.Multiple(() =>
        {
            Assert.That(reports.Count(), Is.EqualTo(numberReports));
            Assert.That(finalValue, Is.EqualTo(expectedEndValue));
            Assert.That(actualTrades.TotalTrades, Is.EqualTo(expectedNumberTrades));
            Assert.That(actualTrades.TotalBuyTrades, Is.EqualTo(expectedBuyTrades));
            Assert.That(actualTrades.TotalSellTrades, Is.EqualTo(expectedSellTrades));
            if (expectedTrades.Count > 0)
            {
                Assert.That(actualTrades.DailyTrades, Is.EquivalentTo(expectedTrades));
            }
        });
    }
}