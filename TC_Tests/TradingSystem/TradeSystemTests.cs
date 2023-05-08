using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Common.Structure.Reporting;

using FinancialStructures.Database;
using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.StockStructures.Statistics;

using NUnit.Framework;

using TradingConsole.TradingSystem;

using TradingSystem.DecideThenTradeSystem;
using TradingSystem.Decisions;
using TradingSystem.Trading;

namespace TradingConsole.Tests.TradingSystem
{
    [TestFixture]
    internal sealed class TradeSystemTests
    {
        public static IEnumerable<TestCaseData> TradeSystemCases()
        {
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                17389.4094462585454082m,
                16,
                16,
                0)
                .SetName("BuyAll-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2017, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                13637.6561m,
                15,
                15,
                0)
                .SetName("BuyAll-2017-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22981.630923004150446m,
                94,
                70,
                24)
                .SetName("FiveDayStatsLeastSquares-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                21384.0343847656261407m,
                197,
                147,
                50)
                .SetName("FiveDayStatsLasso-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22981.630923004150446m,
                94,
                70,
                24)
                .SetName("FiveDayStatsRidge-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20698.6897747802738038m,
                15,
                11,
                4)
                .SetName("FiveDayStatsLeastSquares-2015-2019-hardBuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20339.7689443969733093m,
                47,
                33,
                14)
                .SetName("FiveDayStatsLasso-2015-2019-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20698.6897747802738038m,
                15,
                11,
                4)
                .SetName("FiveDayStatsRidge-2015-2019-hardbuy");

            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18956.283569488525106m,
                43,
                31,
                12)
                .SetName("FiveDayStatsLeastSquares-2016-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19803.945m,
                4,
                2,
                2)
                .SetName("FiveDayStatsLeastSquares-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18196.5226364135759164m,
                81,
                58,
                23)
                .SetName("FiveDayStatsLasso-2016-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19702.815m,
                10,
                7,
                3)
                .SetName("FiveDayStatsLasso-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18956.283569488525106m,
                43,
                31,
                12)
                .SetName("FiveDayStatsRidge-2016-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19803.845m,
                4,
                2,
                2)
                .SetName("FiveDayStatsRidge-2016-2018-hardbuy");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18785.4859167480495155m,
                22,
                13,
                9)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19311.252608642577984m,
                18,
                11,
                7)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy-5daylater");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18638.3596334838838717m,
                221,
                157,
                64)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-5 day later");
        }

        [TestCaseSource(nameof(TradeSystemCases))]
        public void RunTradeSystem(
            string databaseName,
            DecisionSystem decisions,
            List<StockStatisticType> stockStatistics,
            int dayAfterPredictor,
            double buyThreshold,
            double sellThreshold,
            DateTime startTime,
            DateTime endDate,
            decimal expectedEndValue,
            int expectedNumberTrades,
            int expectedBuyTrades,
            int expectedSellTrades)
        {
            decimal tol = 1e-2m;
            var portfolioStartSettings = new PortfolioStartSettings(null, startTime, 20000);
            var decisionParameters = new DecisionSystemFactory.Settings(decisions, stockStatistics, buyThreshold, sellThreshold, dayAfterPredictor);
            var fileSystem = new MockFileSystem();
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, databaseName));
            string testFilePath = "c:/temp/exampleFile.xml";
            fileSystem.AddFile(testFilePath, configureFile);

            void reportAction(ReportSeverity severity, ReportType reportType, string location, string text)
            {
            }

            var logger = new LogReporter(reportAction, saveInternally: true);
            var output = TradeSystem.SetupAndSimulate(
                testFilePath,
                startTime,
                endDate,
                TimeSpan.FromDays(1),
                portfolioStartSettings,
                decisionParameters,
                new TradeMechanismTraderOptions(),
                TradeMechanismType.SellAllThenBuy,
                fileSystem,
                logger);
            var portfolio = output.Portfolio;
            var trades = output.Trades;

            Assert.Multiple(() =>
            {
                Assert.That(20000 - portfolio.TotalValue(Totals.All, startTime.AddDays(-1)), Is.LessThan(tol), "Start value not correct.");
                decimal finalValue = portfolio.TotalValue(Totals.All, endDate);
                Assert.That(expectedEndValue - finalValue, Is.LessThan(tol), $"End value not correct. Expected {expectedEndValue} but was {finalValue}");
                Assert.AreEqual(expectedNumberTrades, trades.TotalTrades, "Number of trades wrong");
                Assert.AreEqual(expectedBuyTrades, trades.TotalBuyTrades, "Number of buy trades wrong.");
                Assert.AreEqual(expectedSellTrades, trades.TotalSellTrades, "Number of sell trades wrong.");
            });

        }
    }
}
