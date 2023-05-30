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
            string barcName = "-Barclays";
            string dunelmName = "stuff-Dunelm";
            string tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2015-01-05T08:00:00|2015-01-05T08:00:00|{barcName}|Buy|20|
|2015-01-05T08:00:00|2015-01-05T08:00:00|{dunelmName}|Buy|4|
|2015-01-06T08:00:00|2015-01-06T08:00:00|{barcName}|Buy|11|
|2015-01-06T08:00:00|2015-01-06T08:00:00|{dunelmName}|Buy|2|
|2015-01-07T08:00:00|2015-01-07T08:00:00|{barcName}|Buy|7|
|2015-01-07T08:00:00|2015-01-07T08:00:00|{dunelmName}|Buy|1|
|2015-01-08T08:00:00|2015-01-08T08:00:00|{barcName}|Buy|4|
|2015-01-09T08:00:00|2015-01-09T08:00:00|{barcName}|Buy|3
|2015-01-12T08:00:00|2015-01-12T08:00:00|{barcName}|Buy|2|
|2015-01-13T08:00:00|2015-01-13T08:00:00|{barcName}|Buy|2|
|2015-01-14T08:00:00|2015-01-14T08:00:00|{barcName}|Buy|1|
|2015-01-15T08:00:00|2015-01-15T08:00:00|{barcName}|Buy|1|
|2015-01-16T08:00:00|2015-01-16T08:00:00|{barcName}|Buy|1|
|2015-01-19T08:00:00|2015-01-19T08:00:00|{barcName}|Buy|1|
|2016-01-12T08:00:00|2016-01-12T08:00:00|{barcName}|Buy|1|
|2016-03-31T08:00:00|2016-03-31T08:00:00|{barcName}|Buy|1|";
            var trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                17389.4094462585454082m,
                16,
                16,
                0,
                trades)
                .SetName("BuyAll-2015-2019");
            tradeString =
    @$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-01-05T08:00:00|2017-01-05T08:00:00|{barcName}|Buy|21|
|2017-01-05T08:00:00|2017-01-05T08:00:00|{dunelmName}|Buy|4|
|2017-01-06T08:00:00|2017-01-06T08:00:00|{barcName}|Buy|12|
|2017-01-06T08:00:00|2017-01-06T08:00:00|{dunelmName}|Buy|2|
|2017-01-09T08:00:00|2017-01-09T08:00:00|{barcName}|Buy|8|
|2017-01-09T08:00:00|2017-01-09T08:00:00|{dunelmName}|Buy|1|
|2017-01-10T08:00:00|2017-01-10T08:00:00|{barcName}|Buy|5|
|2017-01-10T08:00:00|2017-01-10T08:00:00|{dunelmName}|Buy|1|
|2017-01-11T08:00:00|2017-01-11T08:00:00|{barcName}|Buy|3|
|2017-01-12T08:00:00|2017-01-12T08:00:00|{barcName}|Buy|2|
|2017-01-13T08:00:00|2017-01-13T08:00:00|{barcName}|Buy|1|
|2017-01-16T08:00:00|2017-01-16T08:00:00|{barcName}|Buy|1|
|2017-01-17T08:00:00|2017-01-17T08:00:00|{barcName}|Buy|1|
|2017-01-19T08:00:00|2017-01-19T08:00:00|{barcName}|Buy|1|
|2018-09-17T08:00:00|2018-09-17T08:00:00|{barcName}|Buy|1|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2017, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                13637.6561m,
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
                22981.630923004150446m,
                94,
                70,
                24,
                new Dictionary<DateTime, TradeCollection>())
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
                50,
                new Dictionary<DateTime, TradeCollection>())
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
                24,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-2015-2019");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2018-12-07T08:00:00|2018-12-07T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-11T08:00:00|2018-12-11T08:00:00|stuff-Dunelm|Sell|8|
|2019-01-08T08:00:00|2019-01-08T08:00:00|stuff-Dunelm|Buy|7|
|2019-01-09T08:00:00|2019-01-09T08:00:00|stuff-Dunelm|Buy|5|
|2019-01-10T08:00:00|2019-01-10T08:00:00|stuff-Dunelm|Buy|4|
|2019-01-11T08:00:00|2019-01-11T08:00:00|stuff-Dunelm|Buy|3|
|2019-01-16T08:00:00|2019-01-16T08:00:00|stuff-Dunelm|Sell|19|
|2019-10-16T08:00:00|2019-10-16T08:00:00|Barclays|Buy|30|
|2019-10-17T08:00:00|2019-10-17T08:00:00|Barclays|Buy|22|
|2019-10-18T08:00:00|2019-10-18T08:00:00|stuff-Dunelm|Buy|3|
|2019-10-25T08:00:00|2019-10-25T08:00:00|stuff-Dunelm|Sell|3|
|2019-11-04T08:00:00|2019-11-04T08:00:00|Barclays|Sell|52.0|
|2019-12-09T08:00:00|2019-12-09T08:00:00|stuff-Dunelm|Buy|5|
|2019-12-10T08:00:00|2019-12-10T08:00:00|stuff-Dunelm|Buy|3|
|2019-12-11T08:00:00|2019-12-11T08:00:00|stuff-Dunelm|Buy|2|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20698.6897747802738038m,
                15,
                11,
                4,
                trades)
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
                14,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLasso-2015-2019-hardbuy");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2018-12-07T08:00:00|2018-12-07T08:00:00|stuff-Dunelm|Buy|8|
|2018-12-11T08:00:00|2018-12-11T08:00:00|stuff-Dunelm|Sell|8|
|2019-01-08T08:00:00|2019-01-08T08:00:00|stuff-Dunelm|Buy|7|
|2019-01-09T08:00:00|2019-01-09T08:00:00|stuff-Dunelm|Buy|5|
|2019-01-10T08:00:00|2019-01-10T08:00:00|stuff-Dunelm|Buy|4|
|2019-01-11T08:00:00|2019-01-11T08:00:00|stuff-Dunelm|Buy|3|
|2019-01-16T08:00:00|2019-01-16T08:00:00|stuff-Dunelm|Sell|19|
|2019-10-16T08:00:00|2019-10-16T08:00:00|Barclays|Buy|30|
|2019-10-17T08:00:00|2019-10-17T08:00:00|Barclays|Buy|22|
|2019-10-18T08:00:00|2019-10-18T08:00:00|stuff-Dunelm|Buy|3|
|2019-10-25T08:00:00|2019-10-25T08:00:00|stuff-Dunelm|Sell|3|
|2019-11-04T08:00:00|2019-11-04T08:00:00|Barclays|Sell|52.0|
|2019-12-09T08:00:00|2019-12-09T08:00:00|stuff-Dunelm|Buy|5|
|2019-12-10T08:00:00|2019-12-10T08:00:00|stuff-Dunelm|Buy|3|
|2019-12-11T08:00:00|2019-12-11T08:00:00|stuff-Dunelm|Buy|2|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2015, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20698.6897747802738038m,
                15,
                11,
                4,
                trades)
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
                12,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsLeastSquares-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-18T08:00:00|2017-09-18T08:00:00|{dunelmName}|Buy|7|
|2017-09-21T08:00:00|2017-09-21T08:00:00|{dunelmName}|Sell|7|
|2018-12-07T08:00:00|2018-12-07T08:00:00|{dunelmName}|Buy|8|
|2018-12-11T08:00:00|2018-12-11T08:00:00|{dunelmName}|Sell|8|";
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
|2017-09-18T08:00:00|2017-09-18T08:00:00|{dunelmName}|Buy|7|
|2017-09-19T08:00:00|2017-09-19T08:00:00|{dunelmName}|Buy|5|
|2017-09-21T08:00:00|2017-09-21T08:00:00|{dunelmName}|Sell|12|
|2018-09-17T08:00:00|2018-09-17T08:00:00|{dunelmName}|Buy|8|
|2018-09-18T08:00:00|2018-09-18T08:00:00|{dunelmName}|Buy|6|
|2018-09-20T08:00:00|2018-09-20T08:00:00|{dunelmName}|Sell|14|
|2018-12-06T08:00:00|2018-12-06T08:00:00|{dunelmName}|Buy|8|
|2018-12-07T08:00:00|2018-12-07T08:00:00|{dunelmName}|Buy|6|
|2018-12-10T08:00:00|2018-12-10T08:00:00|{dunelmName}|Buy|4|
|2018-12-11T08:00:00|2018-12-11T08:00:00|{dunelmName}|Sell|18|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19702.815m,
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
                18956.283569488525106m,
                43,
                31,
                12,
                new Dictionary<DateTime, TradeCollection>())
                .SetName("FiveDayStatsRidge-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-18T08:00:00|2017-09-18T08:00:00|{dunelmName}|Buy|7|
|2017-09-21T08:00:00|2017-09-21T08:00:00|{dunelmName}|Sell|7|
|2018-12-07T08:00:00|2018-12-07T08:00:00|{dunelmName}|Buy|8|
|2018-12-11T08:00:00|2018-12-11T08:00:00|{dunelmName}|Sell|8|";
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
|2017-09-25T08:00:00|2017-09-25T08:00:00|Johnson Matthew-|Buy|1|
|2017-09-26T08:00:00|2017-09-26T08:00:00|Johnson Matthew-|Buy|1|
|2017-10-02T08:00:00|2017-10-02T08:00:00|Johnson Matthew-|Sell|2|
|2018-04-13T08:00:00|2018-04-13T08:00:00|Tesco-|Buy|17|
|2018-04-16T08:00:00|2018-04-16T08:00:00|Tesco-|Buy|12|
|2018-04-17T08:00:00|2018-04-17T08:00:00|Tesco-|Buy|9|
|2018-04-24T08:00:00|2018-04-24T08:00:00|Glencore-|Buy|5|
|2018-04-26T08:00:00|2018-04-26T08:00:00|Tesco-|Sell|38.0|
|2018-04-27T08:00:00|2018-04-27T08:00:00|Glencore-|Sell|5.0|
|2018-06-06T08:00:00|2018-06-06T08:00:00|Johnson Matthew-|Buy|1|
|2018-06-07T08:00:00|2018-06-07T08:00:00|Johnson Matthew-|Buy|1|
|2018-06-19T08:00:00|2018-06-19T08:00:00|Johnson Matthew-|Sell|2|
|2018-09-24T08:00:00|2018-09-24T08:00:00|Glencore-|Buy|15|
|2018-10-02T08:00:00|2018-10-02T08:00:00|Glencore-|Sell|15|
|2018-11-05T08:00:00|2018-11-05T08:00:00|Glencore-|Buy|15|
|2018-11-08T08:00:00|2018-11-08T08:00:00|Glencore-|Sell|15|
|2018-11-13T08:00:00|2018-11-13T08:00:00|Associated British Foods-|Buy|1|
|2018-11-16T08:00:00|2018-11-16T08:00:00|Associated British Foods-|Sell|1|
|2018-12-04T08:00:00|2018-12-04T08:00:00|Glencore-|Buy|15|
|2018-12-07T08:00:00|2018-12-07T08:00:00|Glencore-|Sell|15|
|2018-12-07T08:00:00|2018-12-07T08:00:00|Dunelm-|Buy|7|
|2018-12-11T08:00:00|2018-12-11T08:00:00|Dunelm-|Sell|7|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                18785.4859167480495155m,
                22,
                13,
                9,
                trades)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy");

            tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-09-19T08:00:00|2017-09-19T08:00:00|Dunelm-|Buy|7|
|2017-09-21T08:00:00|2017-09-21T08:00:00|Dunelm-|Sell|7.0|
|2017-09-25T08:00:00|2017-09-25T08:00:00|Johnson Matthew-|Buy|1|
|2017-09-26T08:00:00|2017-09-26T08:00:00|Johnson Matthew-|Buy|1|
|2017-10-23T08:00:00|2017-10-23T08:00:00|Johnson Matthew-|Sell|2.0|
|2018-04-16T08:00:00|2018-04-16T08:00:00|Tesco-|Buy|16|
|2018-04-17T08:00:00|2018-04-17T08:00:00|Tesco-|Buy|12|
|2018-04-24T08:00:00|2018-04-24T08:00:00|Glencore-|Buy|7|
|2018-05-01T08:00:00|2018-05-01T08:00:00|Glencore-|Sell|7.0|
|2018-07-03T08:00:00|2018-07-03T08:00:00|Tesco-|Sell|28.0|
|2018-09-18T08:00:00|2018-09-18T08:00:00|Dunelm-|Buy|9|
|2018-09-20T08:00:00|2018-09-20T08:00:00|Dunelm-|Sell|9.00|
|2018-09-24T08:00:00|2018-09-24T08:00:00|Glencore-|Buy|15|
|2018-10-03T08:00:00|2018-10-03T08:00:00|Glencore-|Sell|15.00|
|2018-11-13T08:00:00|2018-11-13T08:00:00|Associated British Foods-|Buy|1|
|2018-11-16T08:00:00|2018-11-16T08:00:00|Associated British Foods-|Sell|1.0|
|2018-12-07T08:00:00|2018-12-07T08:00:00|Dunelm-|Buy|8|
|2018-12-10T08:00:00|2018-12-10T08:00:00|Dunelm-|Buy|6|";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.1, 1.0,
                new DateTime(2016, 1, 5, 8, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19311.252608642577984m,
                18,
                11,
                7,
                trades)
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
