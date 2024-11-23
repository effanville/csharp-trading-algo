using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;

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
|2015-01-05T08:00:00|2015-01-05T08:00:00|-Barclays|Buy|20|
|2015-01-05T08:00:00|2015-01-05T08:00:00|stuff-Dunelm|Buy|4|
|2015-01-06T08:00:00|2015-01-06T08:00:00|-Barclays|Buy|12|
|2015-01-06T08:00:00|2015-01-06T08:00:00|stuff-Dunelm|Buy|2|
|2015-01-07T08:00:00|2015-01-07T08:00:00|-Barclays|Buy|7|
|2015-01-07T08:00:00|2015-01-07T08:00:00|stuff-Dunelm|Buy|1|
|2015-01-08T08:00:00|2015-01-08T08:00:00|-Barclays|Buy|4|
|2015-01-09T08:00:00|2015-01-09T08:00:00|-Barclays|Buy|3|
|2015-01-12T08:00:00|2015-01-12T08:00:00|-Barclays|Buy|2|
|2015-01-13T08:00:00|2015-01-13T08:00:00|-Barclays|Buy|2|
|2015-01-14T08:00:00|2015-01-14T08:00:00|-Barclays|Buy|1|
|2015-01-15T08:00:00|2015-01-15T08:00:00|-Barclays|Buy|1|
|2015-01-16T08:00:00|2015-01-16T08:00:00|-Barclays|Buy|1|
|2015-01-19T08:00:00|2015-01-19T08:00:00|-Barclays|Buy|1|
|2016-01-15T08:00:00|2016-01-15T08:00:00|-Barclays|Buy|1|
|2016-06-24T08:00:00|2016-06-24T08:00:00|-Barclays|Buy|1|";
            var trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                17368.1519532775883712m,
                16,
                16,
                0,
                trades)
                .SetName("BuyAll-2015-2019");
            tradeString =
    @$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-01-05T08:00:00|2017-01-05T08:00:00|-Barclays|Buy|21|
|2017-01-05T08:00:00|2017-01-05T08:00:00|stuff-Dunelm|Buy|4|
|2017-01-06T08:00:00|2017-01-06T08:00:00|-Barclays|Buy|12|
|2017-01-06T08:00:00|2017-01-06T08:00:00|stuff-Dunelm|Buy|2|
|2017-01-09T08:00:00|2017-01-09T08:00:00|-Barclays|Buy|7|
|2017-01-09T08:00:00|2017-01-09T08:00:00|stuff-Dunelm|Buy|1|
|2017-01-10T08:00:00|2017-01-10T08:00:00|-Barclays|Buy|5|
|2017-01-10T08:00:00|2017-01-10T08:00:00|stuff-Dunelm|Buy|1|
|2017-01-11T08:00:00|2017-01-11T08:00:00|-Barclays|Buy|3|
|2017-01-12T08:00:00|2017-01-12T08:00:00|-Barclays|Buy|2|
|2017-01-13T08:00:00|2017-01-13T08:00:00|-Barclays|Buy|2|
|2017-01-16T08:00:00|2017-01-16T08:00:00|-Barclays|Buy|1|
|2017-01-17T08:00:00|2017-01-17T08:00:00|-Barclays|Buy|1|
|2017-02-02T08:00:00|2017-02-02T08:00:00|-Barclays|Buy|1|
|2018-10-15T08:00:00|2018-10-15T08:00:00|-Barclays|Buy|1|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2017, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                13631.6481098m,
                15,
                15,
                0,
                trades)
                .SetName("BuyAll-2017-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22528.7957264709473127m,
                99,
                74,
                25,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLeastSquares-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20339.7689443969733093m,
                267,
                201,
                66,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22431.0045445251465375m,
                99,
                74,
                25,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-2015-2019");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|8|
|2019-01-07T08:00:00|2019-01-07T08:00:00|stuff-Dunelm|Buy|7|
|2019-01-08T08:00:00|2019-01-08T08:00:00|stuff-Dunelm|Buy|5|
|2019-01-09T08:00:00|2019-01-09T08:00:00|stuff-Dunelm|Buy|4|
|2019-01-10T08:00:00|2019-01-10T08:00:00|stuff-Dunelm|Buy|3|
|2019-01-15T08:00:00|2019-01-15T08:00:00|stuff-Dunelm|Sell|19|
|2019-02-20T08:00:00|2019-02-20T08:00:00|stuff-Dunelm|Buy|6|
|2019-02-26T08:00:00|2019-02-26T08:00:00|stuff-Dunelm|Sell|6|
|2019-10-15T08:00:00|2019-10-15T08:00:00|-Barclays|Buy|31|
|2019-10-16T08:00:00|2019-10-16T08:00:00|-Barclays|Buy|22|
|2019-10-17T08:00:00|2019-10-17T08:00:00|stuff-Dunelm|Buy|3|
|2019-10-24T08:00:00|2019-10-24T08:00:00|stuff-Dunelm|Sell|3|
|2019-11-01T08:00:00|2019-11-01T08:00:00|-Barclays|Sell|53|
|2019-12-06T08:00:00|2019-12-06T08:00:00|stuff-Dunelm|Buy|5|
|2019-12-09T08:00:00|2019-12-09T08:00:00|stuff-Dunelm|Buy|3|
|2019-12-10T08:00:00|2019-12-10T08:00:00|stuff-Dunelm|Buy|2|
|2019-12-11T08:00:00|2019-12-11T08:00:00|stuff-Dunelm|Buy|2|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20182.8519476318365826m,
                18,
                13,
                5,
                trades)
                .SetName("FiveDayStatsLeastSquares-2015-2019-hardBuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20182.8519476318365826m,
                78,
                57,
                21,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2015-2019-hardbuy");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|8|
|2019-01-07T08:00:00|2019-01-07T08:00:00|stuff-Dunelm|Buy|7|
|2019-01-08T08:00:00|2019-01-08T08:00:00|stuff-Dunelm|Buy|5|
|2019-01-09T08:00:00|2019-01-09T08:00:00|stuff-Dunelm|Buy|4|
|2019-01-10T08:00:00|2019-01-10T08:00:00|stuff-Dunelm|Buy|3|
|2019-01-15T08:00:00|2019-01-15T08:00:00|stuff-Dunelm|Sell|19|
|2019-02-20T08:00:00|2019-02-20T08:00:00|stuff-Dunelm|Buy|6|
|2019-02-26T08:00:00|2019-02-26T08:00:00|stuff-Dunelm|Sell|6|
|2019-10-15T08:00:00|2019-10-15T08:00:00|-Barclays|Buy|31|
|2019-10-16T08:00:00|2019-10-16T08:00:00|-Barclays|Buy|22|
|2019-10-17T08:00:00|2019-10-17T08:00:00|stuff-Dunelm|Buy|3|
|2019-10-24T08:00:00|2019-10-24T08:00:00|stuff-Dunelm|Sell|3|
|2019-11-01T08:00:00|2019-11-01T08:00:00|-Barclays|Sell|53|
|2019-12-06T08:00:00|2019-12-06T08:00:00|stuff-Dunelm|Buy|5|
|2019-12-09T08:00:00|2019-12-09T08:00:00|stuff-Dunelm|Buy|3|
|2019-12-10T08:00:00|2019-12-10T08:00:00|stuff-Dunelm|Buy|2|
|2019-12-11T08:00:00|2019-12-11T08:00:00|stuff-Dunelm|Buy|2|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20698.6897747802738038m,
                18,
                13,
                5,
                trades)
                .SetName("FiveDayStatsRidge-2015-2019-hardbuy");

            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18747.796563110351293m,
                44,
                32,
                12,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLeastSquares-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-15T08:00:00|2017-09-15T08:00:00|stuff-Dunelm|Buy|7|
|2017-09-20T08:00:00|2017-09-20T08:00:00|stuff-Dunelm|Sell|7|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|8|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19803.945m,
                4,
                2,
                2,
                trades)
                .SetName("FiveDayStatsLeastSquares-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18196.5226364135759164m,
                94,
                69,
                25,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-08-07T08:00:00|2017-08-07T08:00:00|stuff-Dunelm|Buy|7|
|2017-08-10T08:00:00|2017-08-10T08:00:00|stuff-Dunelm|Sell|7|
|2017-09-15T08:00:00|2017-09-15T08:00:00|stuff-Dunelm|Buy|7|
|2017-09-18T08:00:00|2017-09-18T08:00:00|stuff-Dunelm|Buy|5|
|2017-09-20T08:00:00|2017-09-20T08:00:00|stuff-Dunelm|Sell|12|
|2018-09-14T08:00:00|2018-09-14T08:00:00|stuff-Dunelm|Buy|9|
|2018-09-17T08:00:00|2018-09-17T08:00:00|stuff-Dunelm|Buy|6|
|2018-09-19T08:00:00|2018-09-19T08:00:00|stuff-Dunelm|Sell|15|
|2018-10-16T08:00:00|2018-10-16T08:00:00|stuff-Dunelm|Buy|8|
|2018-10-22T08:00:00|2018-10-22T08:00:00|stuff-Dunelm|Sell|8|
|2018-12-05T08:00:00|2018-12-05T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|6|
|2018-12-07T08:00:00|2018-12-07T08:00:00|stuff-Dunelm|Buy|4|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|18|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19442.675m,
                14,
                9,
                5,
                trades)
                .SetName("FiveDayStatsLasso-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18747.796563110351293m,
                44,
                32,
                12,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-15T08:00:00|2017-09-15T08:00:00|stuff-Dunelm|Buy|7|
|2017-09-20T08:00:00|2017-09-20T08:00:00|stuff-Dunelm|Sell|7|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|8|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19803.845m,
                4,
                2,
                2,
                trades)
                .SetName("FiveDayStatsRidge-2016-2018-hardbuy");
            tradeString =
$@"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-22T08:00:00|2017-09-22T08:00:00|Johnson Matthew-|Buy|1|
|2017-09-25T08:00:00|2017-09-25T08:00:00|Johnson Matthew-|Buy|1|
|2017-09-29T08:00:00|2017-09-29T08:00:00|Johnson Matthew-|Sell|2|
|2018-04-12T08:00:00|2018-04-12T08:00:00|Tesco-|Buy|16|
|2018-04-13T08:00:00|2018-04-13T08:00:00|Tesco-|Buy|12|
|2018-04-16T08:00:00|2018-04-16T08:00:00|Tesco-|Buy|9|
|2018-04-23T08:00:00|2018-04-23T08:00:00|Glencore-|Buy|6|
|2018-04-25T08:00:00|2018-04-25T08:00:00|Tesco-|Sell|37|
|2018-04-26T08:00:00|2018-04-26T08:00:00|Glencore-|Sell|6|
|2018-06-05T08:00:00|2018-06-05T08:00:00|Johnson Matthew-|Buy|1|
|2018-06-06T08:00:00|2018-06-06T08:00:00|Johnson Matthew-|Buy|1|
|2018-06-18T08:00:00|2018-06-18T08:00:00|Johnson Matthew-|Sell|2|
|2018-09-21T08:00:00|2018-09-21T08:00:00|Glencore-|Buy|15|
|2018-10-01T08:00:00|2018-10-01T08:00:00|Glencore-|Sell|15|
|2018-11-02T08:00:00|2018-11-02T08:00:00|Glencore-|Buy|14|
|2018-11-07T08:00:00|2018-11-07T08:00:00|Glencore-|Sell|14|
|2018-11-12T08:00:00|2018-11-12T08:00:00|Associated British Foods-|Buy|1|
|2018-11-15T08:00:00|2018-11-15T08:00:00|Associated British Foods-|Sell|1|
|2018-12-03T08:00:00|2018-12-03T08:00:00|Glencore-|Buy|15|
|2018-12-06T08:00:00|2018-12-06T08:00:00|Glencore-|Sell|15|
|2018-12-06T08:00:00|2018-12-06T08:00:00|Dunelm-|Buy|7|
|2018-12-10T08:00:00|2018-12-10T08:00:00|Dunelm-|Sell|7|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18762.1630072021498325m,
                22,
                13,
                9,
                trades)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy");

            tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-18T08:00:00|2017-09-18T08:00:00|Dunelm-|Buy|7|
|2017-09-20T08:00:00|2017-09-20T08:00:00|Dunelm-|Sell|7.0|
|2017-09-22T08:00:00|2017-09-22T08:00:00|Johnson Matthew-|Buy|1|
|2017-09-25T08:00:00|2017-09-25T08:00:00|Johnson Matthew-|Buy|1|
|2017-10-20T08:00:00|2017-10-20T08:00:00|Johnson Matthew-|Sell|2.0|
|2018-04-13T08:00:00|2018-04-13T08:00:00|Tesco-|Buy|16|
|2018-04-16T08:00:00|2018-04-16T08:00:00|Tesco-|Buy|12|
|2018-04-23T08:00:00|2018-04-23T08:00:00|Glencore-|Buy|7|
|2018-04-30T08:00:00|2018-04-30T08:00:00|Glencore-|Sell|7.0|
|2018-07-02T08:00:00|2018-07-02T08:00:00|Tesco-|Sell|28.0|
|2018-09-17T08:00:00|2018-09-17T08:00:00|Dunelm-|Buy|9|
|2018-09-19T08:00:00|2018-09-19T08:00:00|Dunelm-|Sell|9.00|
|2018-09-21T08:00:00|2018-09-21T08:00:00|Glencore-|Buy|15|
|2018-10-02T08:00:00|2018-10-02T08:00:00|Glencore-|Sell|15.00|
|2018-11-12T08:00:00|2018-11-12T08:00:00|Associated British Foods-|Buy|1|
|2018-11-15T08:00:00|2018-11-15T08:00:00|Associated British Foods-|Sell|1.0|
|2018-12-06T08:00:00|2018-12-06T08:00:00|Dunelm-|Buy|8|
|2018-12-07T08:00:00|2018-12-07T08:00:00|Dunelm-|Buy|6|
|2018-12-11T08:00:00|2018-12-11T08:00:00|Dunelm-|Sell|14.000|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.1, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19261.28120025634714m,
                19,
                11,
                8,
                trades)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy-5daylater");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.05, 1.0,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18125.4970196533177773m,
                223,
                158,
                65,
                new Dictionary<DateTime, TradeCollection>())
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

            var logger = new LogReporter(null, new SingleTaskQueue(),  saveInternally: true);
            var output = TradingSystemRegistration.SetupAndRun(
                testFilePath,
                startTime,
                endDate,
                TimeSpan.FromDays(1),
                portfolioStartSettings,
                PortfolioConstructionSettings.Default(),
                decisionParameters,
                fileSystem,
                logger);
            var portfolio = output.Portfolio;
            var trades = output.Trades;

            logger.WriteReportsToFile($"logs\\{DateTime.Now:yyyy-MM-ddTHHmmss}{TestContext.CurrentContext.Test.Name}.log");
            Assert.Multiple(() =>
            {
                Assert.That(20000 - portfolio.TotalValue(Totals.All, startTime.AddDays(-1)), Is.LessThan(tol), "Start value not correct.");
                decimal finalValue = portfolio.TotalValue(Totals.All, endDate);
                Assert.That(expectedEndValue - finalValue, Is.LessThan(tol), $"End value not correct. Expected {expectedEndValue} but was {finalValue}");
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
