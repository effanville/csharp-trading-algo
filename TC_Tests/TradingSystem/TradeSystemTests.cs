using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Common.Structure.Reporting;

using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.StockStructures.Statistics;

using NUnit.Framework;

using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using TradingConsole.TradingSystem;

using TradingSystem.DecideThenTradeSystem;

namespace TradingConsole.Tests.TradingSystem
{
    [TestFixture]
    public sealed class TradeSystemTests
    {
        public static IEnumerable<TestCaseData> TradeSystemCases()
        {
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0),
                new DateTime(2019, 12, 12, 8, 0, 0),
                17389.4094462585454082m,
                16,
                16,
                0)
                .SetName("BuyAll-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2017, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                13734.633120880126325m,
                15,
                15,
                0)
                .SetName("BuyAll-2017-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0),
                new DateTime(2019, 12, 12, 8, 0, 0),
                22981.630923004150446m,
                94,
                70,
                24)
                .SetName("FiveDayStatsLeastSquares-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0),
                new DateTime(2019, 12, 12, 8, 0, 0),
                21384.0343847656261407m,
                196,
                146,
                50)
                .SetName("FiveDayStatsLasso-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0),
                new DateTime(2019, 12, 12, 8, 0, 0),
                22981.630923004150446m,
                94,
                70,
                24)
                .SetName("FiveDayStatsRidge-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0),
                new DateTime(2019, 12, 12, 8, 0, 0),
                20698.6897747802738038m,
                16,
                12,
                4)
                .SetName("FiveDayStatsLeastSquares-2015-2019-hardBuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0),
                new DateTime(2019, 12, 12, 8, 0, 0),
                20339.7689443969733093m,
                50,
                35,
                15)
                .SetName("FiveDayStatsLasso-2015-2019-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0),
                new DateTime(2019, 12, 12, 8, 0, 0),
                20698.6897747802738038m,
                16,
                12,
                4)
                .SetName("FiveDayStatsRidge-2015-2019-hardbuy");

            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19806.4860676574704154m,
                43,
                31,
                12)
                .SetName("FiveDayStatsLeastSquares-2016-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19893.195m,
                4,
                2,
                2)
                .SetName("FiveDayStatsLeastSquares-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19201.2016111755388426m,
                81,
                58,
                23)
                .SetName("FiveDayStatsLasso-2016-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19881.475m,
                10,
                7,
                3)
                .SetName("FiveDayStatsLasso-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19806.4860676574704154m,
                43,
                31,
                12)
                .SetName("FiveDayStatsRidge-2016-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19893.195m,
                4,
                2,
                2)
                .SetName("FiveDayStatsRidge-2016-2018-hardbuy");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                18935.167845153809962m,
                22,
                13,
                9)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19642.836685485839324m,
                15,
                9,
                6)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy-5daylater");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0),
                new DateTime(2018, 12, 12, 8, 0, 0),
                19069.1183523559543545m,
                218,
                154,
                64)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-5 day later");
        }

        [TestCaseSource(nameof(TradeSystemCases))]
        public void RunTradeSystem(
            string databaseName,
            DecisionSystem.DecisionSystem decisions,
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
            var portfolioStartSettings = new PortfolioStartSettings(null, startTime, 20000);
            var decisionParameters = new DecisionSystemFactory.Settings(decisions, stockStatistics, buyThreshold, sellThreshold, dayAfterPredictor);
            var traderOptions = new TradeMechanismTraderOptions(0.25m);
            var fileSystem = new MockFileSystem();
            var configureFile = File.ReadAllText($"{TestConstants.ExampleFilesLocation}\\{databaseName}");
            string testFilePath = "c:/temp/exampleFile.xml";
            fileSystem.AddFile(testFilePath, configureFile);

            var reports = new ErrorReports();
            void reportAction(ReportSeverity severity, ReportType reportType, ReportLocation location, string text)
            {
                reports.AddErrorReport(severity, reportType, location, text);
            }

            var logger = new LogReporter(reportAction);
            var output = TradeSystem.SetupAndSimulate(
                testFilePath,
                startTime,
                endDate,
                TimeSpan.FromDays(1),
                portfolioStartSettings,
                decisionParameters,
                traderOptions,
                TradeMechanismType.SellAllThenBuy,
                fileSystem,
                logger);
            var portfolio = output.Portfolio;
            var trades = output.Trades;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(20000, portfolio.TotalValue(FinancialStructures.Database.Totals.All, startTime.AddDays(-1)));
                Assert.AreEqual(expectedEndValue, portfolio.TotalValue(FinancialStructures.Database.Totals.All, endDate));
                Assert.AreEqual(expectedNumberTrades, trades.TotalTrades());
                Assert.AreEqual(expectedBuyTrades, trades.TotalBuyTrades());
                Assert.AreEqual(expectedSellTrades, trades.TotalSellTrades());
            });

        }
    }
}
