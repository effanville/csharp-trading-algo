using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Strategies;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingSystem.DependencyInjection;

using Microsoft.Extensions.Hosting;

using NUnit.Framework;

using TradingConsole.Tests;

using DecisionSystemFactory = Effanville.TradingStructures.Strategies.Decision.DecisionSystemFactory;

namespace Effanville.TradingSystem.Tests
{
    [TestFixture]
    internal sealed class TradeSystemTests
    {
        public static IEnumerable<TestCaseData> TradeSystemCases()
        {
            string tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-06-26T08:00:00|2017-06-26T08:00:00|-Barclays|Buy|25|
|2017-06-26T08:00:00|2017-06-26T08:00:00|stuff-Dunelm|Buy|5|
|2017-06-27T08:00:00|2017-06-27T08:00:00|-Barclays|Buy|14|
|2017-06-27T08:00:00|2017-06-27T08:00:00|stuff-Dunelm|Buy|3|
|2017-06-28T08:00:00|2017-06-28T08:00:00|-Barclays|Buy|9|
|2017-06-28T08:00:00|2017-06-28T08:00:00|stuff-Dunelm|Buy|2|
|2017-06-29T08:00:00|2017-06-29T08:00:00|-Barclays|Buy|5|
|2017-06-29T08:00:00|2017-06-29T08:00:00|stuff-Dunelm|Buy|1|
|2017-06-30T08:00:00|2017-06-30T08:00:00|-Barclays|Buy|3|
|2017-07-03T08:00:00|2017-07-03T08:00:00|-Barclays|Buy|2|
|2017-07-04T08:00:00|2017-07-04T08:00:00|-Barclays|Buy|1|
|2017-07-05T08:00:00|2017-07-05T08:00:00|-Barclays|Buy|1|
|2017-07-06T08:00:00|2017-07-06T08:00:00|-Barclays|Buy|1|
|2017-07-07T08:00:00|2017-07-07T08:00:00|-Barclays|Buy|1|
|2018-09-17T08:00:00|2018-09-17T08:00:00|-Barclays|Buy|1|";
            var trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22922.298m,
                15,
                15,
                0,
                trades)
                .SetName("BuyAll-2015-2019");
            tradeString =
    @$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-12-27T08:00:00|2017-12-27T08:00:00|-Barclays|Buy|24|
|2017-12-27T08:00:00|2017-12-27T08:00:00|stuff-Dunelm|Buy|5|
|2017-12-28T08:00:00|2017-12-28T08:00:00|-Barclays|Buy|14|
|2017-12-28T08:00:00|2017-12-28T08:00:00|stuff-Dunelm|Buy|3|
|2017-12-29T08:00:00|2017-12-29T08:00:00|-Barclays|Buy|8|
|2017-12-29T08:00:00|2017-12-29T08:00:00|stuff-Dunelm|Buy|1|
|2018-01-02T08:00:00|2018-01-02T08:00:00|-Barclays|Buy|5|
|2018-01-02T08:00:00|2018-01-02T08:00:00|stuff-Dunelm|Buy|1|
|2018-01-03T08:00:00|2018-01-03T08:00:00|-Barclays|Buy|3|
|2018-01-04T08:00:00|2018-01-04T08:00:00|-Barclays|Buy|2|
|2018-01-05T08:00:00|2018-01-05T08:00:00|-Barclays|Buy|2|
|2018-01-08T08:00:00|2018-01-08T08:00:00|-Barclays|Buy|1|
|2018-01-09T08:00:00|2018-01-09T08:00:00|-Barclays|Buy|1|
|2018-01-10T08:00:00|2018-01-10T08:00:00|-Barclays|Buy|1|
|2018-12-05T08:00:00|2018-12-05T08:00:00|-Barclays|Buy|1|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.0, 1.0,
                new DateTime(2017, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                15708.75146m,
                15,
                15,
                0,
                trades)
                .SetName("BuyAll-2017-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.01, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22669.8432600402834084m,
                53,
                36,
                17,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLeastSquares-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.04, 0.98,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                21075.8615869140621937m,
                68,
                48,
                20,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.01, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22669.8432600402834084m,
                53,
                36,
                17,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-2015-2019");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-07-11T08:00:00|2017-07-11T08:00:00|stuff-Dunelm|Buy|8|
|2017-08-09T08:00:00|2017-08-09T08:00:00|stuff-Dunelm|Sell|8.0|
|2018-05-29T08:00:00|2018-05-29T08:00:00|stuff-Dunelm|Buy|9|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|7|
|2018-09-06T08:00:00|2018-09-06T08:00:00|stuff-Dunelm|Sell|16.00|
|2018-12-14T08:00:00|2018-12-14T08:00:00|stuff-Dunelm|Buy|9|
|2018-12-18T08:00:00|2018-12-18T08:00:00|stuff-Dunelm|Buy|8|
|2019-01-04T08:00:00|2019-01-04T08:00:00|stuff-Dunelm|Sell|17.000|
|2019-09-05T08:00:00|2019-09-05T08:00:00|stuff-Dunelm|Buy|6|
|2019-09-06T08:00:00|2019-09-06T08:00:00|stuff-Dunelm|Sell|6.0000|
|2019-09-10T08:00:00|2019-09-10T08:00:00|stuff-Dunelm|Buy|6|
|2019-09-11T08:00:00|2019-09-11T08:00:00|stuff-Dunelm|Sell|6.00000|
|2019-10-11T08:00:00|2019-10-11T08:00:00|stuff-Dunelm|Buy|7|
|2019-10-14T08:00:00|2019-10-14T08:00:00|stuff-Dunelm|Sell|7.000000|
|2019-11-05T08:00:00|2019-11-05T08:00:00|stuff-Dunelm|Buy|7|
|2019-11-28T08:00:00|2019-11-28T08:00:00|stuff-Dunelm|Sell|7.0000000|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.017, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22883.137m,
                16,
                9,
                7,
                trades)
                .SetName("FiveDayStatsLeastSquares-2015-2019-hardBuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.06, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19757.37358m,
                22,
                16,
                6,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2015-2019-hardbuy");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-07-11T08:00:00|2017-07-11T08:00:00|stuff-Dunelm|Buy|8|
|2017-08-09T08:00:00|2017-08-09T08:00:00|stuff-Dunelm|Sell|8.0|
|2018-05-29T08:00:00|2018-05-29T08:00:00|stuff-Dunelm|Buy|9|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|7|
|2018-09-06T08:00:00|2018-09-06T08:00:00|stuff-Dunelm|Sell|16.00|
|2018-12-14T08:00:00|2018-12-14T08:00:00|stuff-Dunelm|Buy|9|
|2018-12-18T08:00:00|2018-12-18T08:00:00|stuff-Dunelm|Buy|8|
|2019-01-04T08:00:00|2019-01-04T08:00:00|stuff-Dunelm|Sell|17.000|
|2019-09-05T08:00:00|2019-09-05T08:00:00|stuff-Dunelm|Buy|6|
|2019-09-06T08:00:00|2019-09-06T08:00:00|stuff-Dunelm|Sell|6.0000|
|2019-09-10T08:00:00|2019-09-10T08:00:00|stuff-Dunelm|Buy|6|
|2019-09-11T08:00:00|2019-09-11T08:00:00|stuff-Dunelm|Sell|6.00000|
|2019-10-11T08:00:00|2019-10-11T08:00:00|stuff-Dunelm|Buy|7|
|2019-10-14T08:00:00|2019-10-14T08:00:00|stuff-Dunelm|Sell|7.000000|
|2019-11-05T08:00:00|2019-11-05T08:00:00|stuff-Dunelm|Buy|7|
|2019-11-28T08:00:00|2019-11-28T08:00:00|stuff-Dunelm|Sell|7.0000000|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.017, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22883.137m,
                16,
                9,
                7,
                trades)
                .SetName("FiveDayStatsRidge-2015-2019-hardbuy");

            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.01, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20257.3864m,
                30,
                23,
                7,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLeastSquares-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2018-05-29T08:00:00|2018-05-29T08:00:00|stuff-Dunelm|Buy|9|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|6|
|2018-09-13T08:00:00|2018-09-13T08:00:00|stuff-Dunelm|Sell|15.0|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.02, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20550.275m,
                3,
                2,
                1,
                trades)
                .SetName("FiveDayStatsLeastSquares-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.015, 0.98,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                15808.3044m,
                69,
                55,
                14,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-15T08:00:00|2017-09-15T08:00:00|stuff-Dunelm|Buy|7|
|2017-09-18T08:00:00|2017-09-18T08:00:00|stuff-Dunelm|Buy|5|
|2017-10-12T08:00:00|2017-10-12T08:00:00|stuff-Dunelm|Buy|4|
|2017-11-03T08:00:00|2017-11-03T08:00:00|stuff-Dunelm|Sell|16|
|2018-04-16T08:00:00|2018-04-16T08:00:00|stuff-Dunelm|Buy|8|
|2018-05-30T08:00:00|2018-05-30T08:00:00|stuff-Dunelm|Sell|8|
|2018-07-17T08:00:00|2018-07-17T08:00:00|stuff-Dunelm|Buy|9|
|2018-08-16T08:00:00|2018-08-16T08:00:00|stuff-Dunelm|Sell|9|
|2018-09-14T08:00:00|2018-09-14T08:00:00|stuff-Dunelm|Buy|9|
|2018-09-17T08:00:00|2018-09-17T08:00:00|stuff-Dunelm|Buy|6|
|2018-09-27T08:00:00|2018-09-27T08:00:00|stuff-Dunelm|Sell|15|
|2018-12-05T08:00:00|2018-12-05T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|6|
|2018-12-12T08:00:00|2018-12-12T08:00:00|stuff-Dunelm|Sell|14|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.03, 0.98,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19335.91m,
                14,
                9,
                5,
                trades)
                .SetName("FiveDayStatsLasso-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.01, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20257.3864m,
                30,
                23,
                7,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2018-05-29T08:00:00|2018-05-29T08:00:00|stuff-Dunelm|Buy|9|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|6|
|2018-09-13T08:00:00|2018-09-13T08:00:00|stuff-Dunelm|Sell|15.0|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.02, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20550.275m,
                3,
                2,
                1,
                trades)
                .SetName("FiveDayStatsRidge-2016-2018-hardbuy");
            tradeString =
$@"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-07-11T08:00:00|2017-07-11T08:00:00|Dunelm-|Buy|8|
|2018-02-08T08:00:00|2018-02-08T08:00:00|Glencore-|Buy|10|
|2018-02-23T08:00:00|2018-02-23T08:00:00|Dunelm-|Buy|5|
|2018-03-22T08:00:00|2018-03-22T08:00:00|Dunelm-|Buy|4|
|2018-03-23T08:00:00|2018-03-23T08:00:00|Glencore-|Buy|4|
|2018-03-27T08:00:00|2018-03-27T08:00:00|Dunelm-|Buy|2|
|2018-05-29T08:00:00|2018-05-29T08:00:00|Dunelm-|Buy|1|
|2018-06-01T08:00:00|2018-06-01T08:00:00|Dunelm-|Buy|1|
|2018-07-04T08:00:00|2018-07-04T08:00:00|Glencore-|Buy|2|
|2018-07-09T08:00:00|2018-07-09T08:00:00|Glencore-|Buy|1|
|2018-07-16T08:00:00|2018-07-16T08:00:00|Glencore-|Buy|1|
|2018-09-18T08:00:00|2018-09-18T08:00:00|Dunelm-|Sell|21.0|
|2018-10-08T08:00:00|2018-10-08T08:00:00|Tesco-|Buy|12|
|2018-10-11T08:00:00|2018-10-11T08:00:00|Glencore-|Buy|8|
|2018-11-16T08:00:00|2018-11-16T08:00:00|3I-|Buy|2|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.012, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18061.07608m,
                15,
                14,
                1,
                trades)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy");

            tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-07-13T08:00:00|2017-07-13T08:00:00|Dunelm-|Buy|9|
|2017-09-14T08:00:00|2017-09-14T08:00:00|Dunelm-|Sell|9.0|
|2018-02-06T08:00:00|2018-02-06T08:00:00|BP-|Buy|11|
|2018-02-21T08:00:00|2018-02-21T08:00:00|Dunelm-|Buy|7|
|2018-02-22T08:00:00|2018-02-22T08:00:00|Dunelm-|Buy|5|
|2018-04-13T08:00:00|2018-04-13T08:00:00|Dunelm-|Sell|12.00|
|2018-05-29T08:00:00|2018-05-29T08:00:00|Dunelm-|Buy|7|
|2018-05-30T08:00:00|2018-05-30T08:00:00|Dunelm-|Buy|5|
|2018-05-31T08:00:00|2018-05-31T08:00:00|Dunelm-|Buy|4|
|2018-07-04T08:00:00|2018-07-04T08:00:00|Glencore-|Buy|5|
|2018-07-16T08:00:00|2018-07-16T08:00:00|Dunelm-|Sell|16.000|
|2018-09-21T08:00:00|2018-09-21T08:00:00|Glencore-|Sell|5.0|
|2018-10-04T08:00:00|2018-10-04T08:00:00|Tesco-|Buy|14|
|2018-12-06T08:00:00|2018-12-06T08:00:00|Glencore-|Buy|10|
|2018-12-07T08:00:00|2018-12-07T08:00:00|Prudential-|Buy|1|
|2018-12-11T08:00:00|2018-12-11T08:00:00|Dunelm-|Buy|3|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.03, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                21168.221423m,
                16,
                12,
                4,
                trades)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy-5daylater");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.01, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19586.833m,
                70,
                63,
                7,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-small-db-2016-2018-5 day later");
        }

        [TestCaseSource(nameof(TradeSystemCases))]
        public async Task RunTradeSystem(
            string databaseName,
            DecisionSystem decisions,
            List<StockStatisticType> stockStatistics,
            int dayAfterPredictor,
            double buyThreshold,
            double sellThreshold,
            DateTime startTime,
            DateTime endTime,
            decimal expectedEndValue,
            int expectedNumberTrades,
            int expectedBuyTrades,
            int expectedSellTrades,
            Dictionary<DateTime, TradeCollection> expectedTrades)
        {
            decimal tol = 1e-2m;
            var portfolioStartSettings = new PortfolioStartSettings("", startTime, 20000);
            var decisionParameters = new DecisionSystemFactory.Settings(decisions, stockStatistics, buyThreshold, sellThreshold, dayAfterPredictor);
            var fileSystem = new MockFileSystem();
            string configureFile = File.ReadAllText(Path.Combine(TestConstants.ExampleFilesLocation, databaseName));
            string testFilePath = "c:/temp/exampleFile.xml";
            fileSystem.AddFile(testFilePath, configureFile);

            var logger = new LogReporter(null, new SingleTaskQueue(), saveInternally: true);

            var builder = new HostApplicationBuilder();
            builder.Logging.RegisterLogging(logger);
            builder.Services.RegisterTradingServices(
                testFilePath,
                startTime,
                endTime,
                TimeSpan.FromDays(1),
                new StrategySettings(portfolioStartSettings,
                PortfolioConstructionSettings.Default(),
                decisionParameters),
                fileSystem);
            var host = builder.Build();
            var output = await host.RunSystemAsync();
            var portfolio = output.Portfolio;
            var trades = output.Trades;

            logger.WriteReportsToFile($"logs\\{DateTime.Now:yyyy-MM-ddTHHmmss}{TestContext.CurrentContext.Test.Name}.log");
            Assert.Multiple(() =>
            {
                Assert.That(portfolio.TotalValue(Totals.All, startTime.AddDays(-1)), Is.EqualTo(20000m).Within(tol), "Start value not correct.");
                decimal finalValue = portfolio.TotalValue(Totals.All, endTime);
                Assert.That(finalValue, Is.EqualTo(expectedEndValue).Within(tol), $"End value not correct. Expected {expectedEndValue} but was {finalValue}");
                Assert.That(trades.TotalTrades, Is.EqualTo(expectedNumberTrades), "Number of trades wrong");
                Assert.That(trades.TotalBuyTrades, Is.EqualTo(expectedBuyTrades), "Number of buy trades wrong.");
                Assert.That(trades.TotalSellTrades, Is.EqualTo(expectedSellTrades), "Number of sell trades wrong.");

                string mdTable = trades.ConvertToTable();
                if (expectedTrades.Count > 0)
                {
                    Assert.That(trades.DailyTrades, Is.EquivalentTo(expectedTrades));
                }
            });

        }
    }
}
