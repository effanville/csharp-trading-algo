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

using TradingSystem.Decisions;
using TradingSystem.PortfolioStrategies;
using TradingSystem.Trading;

namespace TradingConsole.Tests.TradingSystem
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
|2016-06-24T07:00:00|2016-06-24T07:00:00|-Barclays|Buy|1|";
            var trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
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
|2018-10-15T07:00:00|2018-10-15T07:00:00|-Barclays|Buy|1|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2017, 1, 5, 8, 0, 0, DateTimeKind.Utc),
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
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22735.32432571411139m,
                96,
                72,
                24,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLeastSquares-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20339.7689443969733093m,
                199,
                149,
                50,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                22431.0045445251465375m,
                96,
                72,
                24,
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
|2019-10-15T07:00:00|2019-10-15T07:00:00|-Barclays|Buy|31|
|2019-10-16T07:00:00|2019-10-16T07:00:00|-Barclays|Buy|23|
|2019-10-17T07:00:00|2019-10-17T07:00:00|stuff-Dunelm|Buy|3|
|2019-10-24T07:00:00|2019-10-24T07:00:00|stuff-Dunelm|Sell|3|
|2019-11-01T08:00:00|2019-11-01T08:00:00|-Barclays|Sell|54|
|2019-12-06T08:00:00|2019-12-06T08:00:00|stuff-Dunelm|Buy|5|
|2019-12-09T08:00:00|2019-12-09T08:00:00|stuff-Dunelm|Buy|3|
|2019-12-10T08:00:00|2019-12-10T08:00:00|stuff-Dunelm|Buy|2|
|2019-12-11T08:00:00|2019-12-11T08:00:00|stuff-Dunelm|Buy|2|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20182.8519476318365826m,
                16,
                12,
                4,
                trades)
                .SetName("FiveDayStatsLeastSquares-2015-2019-hardBuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20182.8519476318365826m,
                50,
                35,
                15,
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
|2019-10-15T07:00:00|2019-10-15T07:00:00|-Barclays|Buy|31|
|2019-10-16T07:00:00|2019-10-16T07:00:00|-Barclays|Buy|23|
|2019-10-17T07:00:00|2019-10-17T07:00:00|stuff-Dunelm|Buy|3|
|2019-10-24T07:00:00|2019-10-24T07:00:00|stuff-Dunelm|Sell|3|
|2019-11-01T08:00:00|2019-11-01T08:00:00|-Barclays|Sell|54|
|2019-12-06T08:00:00|2019-12-06T08:00:00|stuff-Dunelm|Buy|5|
|2019-12-09T08:00:00|2019-12-09T08:00:00|stuff-Dunelm|Buy|3|
|2019-12-10T08:00:00|2019-12-10T08:00:00|stuff-Dunelm|Buy|2|
|2019-12-11T08:00:00|2019-12-11T08:00:00|stuff-Dunelm|Buy|2|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20698.6897747802738038m,
                16,
                12,
                4,
                trades)
                .SetName("FiveDayStatsRidge-2015-2019-hardbuy");

            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18747.796563110351293m,
                43,
                31,
                12,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLeastSquares-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-15T07:00:00|2017-09-15T07:00:00|stuff-Dunelm|Buy|7|
|2017-09-20T07:00:00|2017-09-20T07:00:00|stuff-Dunelm|Sell|7|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|8|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
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
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18196.5226364135759164m,
                81,
                58,
                23,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-15T07:00:00|2017-09-15T07:00:00|stuff-Dunelm|Buy|7|
|2017-09-18T07:00:00|2017-09-18T07:00:00|stuff-Dunelm|Buy|5|
|2017-09-20T07:00:00|2017-09-20T07:00:00|stuff-Dunelm|Sell|12|
|2018-09-14T07:00:00|2018-09-14T07:00:00|stuff-Dunelm|Buy|8|
|2018-09-17T07:00:00|2018-09-17T07:00:00|stuff-Dunelm|Buy|6|
|2018-09-19T07:00:00|2018-09-19T07:00:00|stuff-Dunelm|Sell|14|
|2018-12-05T08:00:00|2018-12-05T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|6|
|2018-12-07T08:00:00|2018-12-07T08:00:00|stuff-Dunelm|Buy|4|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|18|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19442.675m,
                10,
                7,
                3,
                trades)
                .SetName("FiveDayStatsLasso-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18747.796563110351293m,
                43,
                31,
                12,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-15T07:00:00|2017-09-15T07:00:00|stuff-Dunelm|Buy|7|
|2017-09-20T07:00:00|2017-09-20T07:00:00|stuff-Dunelm|Sell|7|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-10T08:00:00|2018-12-10T08:00:00|stuff-Dunelm|Sell|8|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
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
|2017-09-22T07:00:00|2017-09-22T07:00:00|Johnson Matthew-|Buy|1|
|2017-09-25T07:00:00|2017-09-25T07:00:00|Johnson Matthew-|Buy|1|
|2017-09-29T07:00:00|2017-09-29T07:00:00|Johnson Matthew-|Sell|2|
|2018-04-12T07:00:00|2018-04-12T07:00:00|Tesco-|Buy|16|
|2018-04-13T07:00:00|2018-04-13T07:00:00|Tesco-|Buy|12|
|2018-04-16T07:00:00|2018-04-16T07:00:00|Tesco-|Buy|9|
|2018-04-23T07:00:00|2018-04-23T07:00:00|Glencore-|Buy|6|
|2018-04-25T07:00:00|2018-04-25T07:00:00|Tesco-|Sell|37|
|2018-04-26T07:00:00|2018-04-26T07:00:00|Glencore-|Sell|6|
|2018-06-05T07:00:00|2018-06-05T07:00:00|Johnson Matthew-|Buy|1|
|2018-06-06T07:00:00|2018-06-06T07:00:00|Johnson Matthew-|Buy|1|
|2018-06-18T07:00:00|2018-06-18T07:00:00|Johnson Matthew-|Sell|2|
|2018-09-21T07:00:00|2018-09-21T07:00:00|Glencore-|Buy|15|
|2018-10-01T07:00:00|2018-10-01T07:00:00|Glencore-|Sell|15|
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
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18762.1630072021498325m,
                22,
                13,
                9,
                trades)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy");

            tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-18T07:00:00|2017-09-18T07:00:00|Dunelm-|Buy|7|
|2017-09-20T07:00:00|2017-09-20T07:00:00|Dunelm-|Sell|7|
|2017-09-22T07:00:00|2017-09-22T07:00:00|Johnson Matthew-|Buy|1|
|2017-09-25T07:00:00|2017-09-25T07:00:00|Johnson Matthew-|Buy|1|
|2017-10-20T07:00:00|2017-10-20T07:00:00|Johnson Matthew-|Sell|2|
|2018-04-13T07:00:00|2018-04-13T07:00:00|Tesco-|Buy|16|
|2018-04-16T07:00:00|2018-04-16T07:00:00|Tesco-|Buy|12|
|2018-04-23T07:00:00|2018-04-23T07:00:00|Glencore-|Buy|7|
|2018-04-30T07:00:00|2018-04-30T07:00:00|Glencore-|Sell|7|
|2018-07-02T07:00:00|2018-07-02T07:00:00|Tesco-|Sell|28|
|2018-09-17T07:00:00|2018-09-17T07:00:00|Dunelm-|Buy|9|
|2018-09-19T07:00:00|2018-09-19T07:00:00|Dunelm-|Sell|9|
|2018-12-06T08:00:00|2018-12-06T08:00:00|Dunelm-|Buy|8|
|2018-12-07T08:00:00|2018-12-07T08:00:00|Dunelm-|Buy|6|
|2018-12-11T08:00:00|2018-12-11T08:00:00|Dunelm-|Sell|14|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19261.28120025634714m,
                15,
                9,
                6,
                trades)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy-5daylater");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.05, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18125.4970196533177773m,
                217,
                153,
                64,
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
                PortfolioConstructionSettings.Default(),
                decisionParameters,
                TradeMechanismSettings.Default(),
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

                string mdTable = trades.ConvertToTable();
                if (expectedTrades.Count > 0)
                {
                    CollectionAssert.AreEquivalent(expectedTrades, trades.DailyTrades);
                }
            });

        }
    }
}
