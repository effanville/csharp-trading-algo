using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Decision.Implementation;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

using TradingConsole.Tests;

namespace Effanville.TradingStructures.Strategies.Tests.Decisions
{
    internal class BasicDecisionSystemTests
    {
        [Test]
        public void DecisionsCorrect()
        {
            var fileSystem = new MockFileSystem();
            string configureFile =
                File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, "example-database.xml"));
            string testFilePath = "c:/temp/exampleFile.xml";
            fileSystem.AddFile(testFilePath, configureFile);

            var logger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);
            var stockExchange = new StockExchangeFactory(Substitute.For<ILogger<StockExchangeFactory>>(), new LoggerFactory(), fileSystem)
                .Create(new(testFilePath));
            var settings = new DecisionSystemFactory.Settings(
                DecisionSystem.FiveDayStatsLeastSquares,
                null,
                1.05,
                1.0,
                1);
            var decisionSystem = new FiveDayStatsDecisionSystem(settings, Substitute.For<ILogger<FiveDayStatsDecisionSystem>>());
            var systemSettings = new DecisionSystemSettings(
                DateTime.SpecifyKind(new DateTime(2015, 1, 1), DateTimeKind.Utc),

                DateTime.SpecifyKind(new DateTime(2015, 2, 1), DateTimeKind.Utc),
                2,
                stockExchange);
            decisionSystem.Calibrate(systemSettings);

            var decision = decisionSystem.Decide(
                DateTime.SpecifyKind(new DateTime(2015, 2, 2), DateTimeKind.Utc),
                stockExchange);
            Assert.That(decision.GetBuyDecisions().Count, Is.EqualTo(0));
            Assert.That(decision.GetSellDecisions().Count, Is.EqualTo(2));
        }
    }
}
