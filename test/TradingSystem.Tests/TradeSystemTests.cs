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
using Effanville.TradingStructures.Strategies.Decision;
using Effanville.TradingStructures.Strategies.Portfolio;
using Effanville.TradingSystem.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
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
|2016-06-24T08:00:00|2016-06-24T08:00:00|-Barclays|Buy|1|
";
            var trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.05, 1.0,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                17381.7464501953129m,
                16,
                16,
                0,
                trades,
                null)
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
|2018-10-15T08:00:00|2018-10-15T08:00:00|-Barclays|Buy|1|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.BuyAll,
                null, 1, 1.0, 1.0,
                new DateTime(2017, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                13666.016039428710293m,
                15,
                15,
                0,
                trades,
                null)
                .SetName("BuyAll-2017-2018");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.01, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20404.95611801147413m,
                279,
                202,
                77,
                new Dictionary<DateTime, TradeCollection>(),
                new double[] { 0.17937281526144488, -0.23886084759317328, -0.004175934040631546, 0.35868771847572134, 0.7064015673354334 })
                .SetName("FiveDayStatsLeastSquares-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.04, 0.98,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                 16712.60900131225m,
                202,
                141,
                61,
                new Dictionary<DateTime, TradeCollection>(),
                new double[] { -0.8755208103240394, -0.19418960075404212, 0.345449972386218, 0.8839255403559035, 0.8385930567007495 })
                .SetName("FiveDayStatsLasso-2015-2019");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.01, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                21004.128388671874m,
                280,
                202,
                78,
                new Dictionary<DateTime, TradeCollection>(),
                new double[] { 0.17890627840336037, -0.2380553714834832, -0.003942906670886259, 0.3587310315651848, 0.7057873056110111 })
                .SetName("FiveDayStatsRidge-2015-2019");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2016-01-27T08:00:00|2016-01-27T08:00:00|-Barclays|Buy|27|
|2016-02-10T08:00:00|2016-02-10T08:00:00|-Barclays|Buy|23|
|2016-02-15T08:00:00|2016-02-15T08:00:00|-Barclays|Buy|17|
|2016-02-19T08:00:00|2016-02-19T08:00:00|-Barclays|Sell|67.0|
|2016-05-04T08:00:00|2016-05-04T08:00:00|-Barclays|Buy|29|
|2016-06-24T08:00:00|2016-06-24T08:00:00|-Barclays|Buy|28|
|2016-06-27T08:00:00|2016-06-27T08:00:00|-Barclays|Sell|57.00|
|2016-06-27T08:00:00|2016-06-27T08:00:00|stuff-Dunelm|Buy|5|
|2016-06-29T08:00:00|2016-06-29T08:00:00|-Barclays|Buy|27|
|2016-06-30T08:00:00|2016-06-30T08:00:00|-Barclays|Sell|27.000|
|2017-01-13T08:00:00|2017-01-13T08:00:00|stuff-Dunelm|Buy|5|
|2017-07-11T08:00:00|2017-07-11T08:00:00|stuff-Dunelm|Buy|4|
|2017-10-11T08:00:00|2017-10-11T08:00:00|stuff-Dunelm|Sell|14.0|
|2018-05-29T08:00:00|2018-05-29T08:00:00|stuff-Dunelm|Buy|9|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|6|
|2018-09-13T08:00:00|2018-09-13T08:00:00|stuff-Dunelm|Sell|15.00|
|2018-12-19T08:00:00|2018-12-19T08:00:00|stuff-Dunelm|Buy|9|
|2019-01-04T08:00:00|2019-01-04T08:00:00|stuff-Dunelm|Sell|9.000|
|2019-09-05T08:00:00|2019-09-05T08:00:00|stuff-Dunelm|Buy|6|
|2019-09-10T08:00:00|2019-09-10T08:00:00|stuff-Dunelm|Buy|4|
|2019-10-11T08:00:00|2019-10-11T08:00:00|stuff-Dunelm|Buy|4|
|2019-10-14T08:00:00|2019-10-14T08:00:00|stuff-Dunelm|Sell|14.0000|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.023, 0.98,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20428.256516723m,
                22,
                15,
                7,
                trades,
                null)
                .SetName("FiveDayStatsLeastSquares-2015-2019-hardBuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.06, 0.99,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20755.42115982m,
                80,
                53,
                27,
                new Dictionary<DateTime, TradeCollection>(),
                null)
                .SetName("FiveDayStatsLasso-2015-2019-hardbuy");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2016-01-27T08:00:00|2016-01-27T08:00:00|-Barclays|Buy|27|
|2016-02-10T08:00:00|2016-02-10T08:00:00|-Barclays|Buy|23|
|2016-02-15T08:00:00|2016-02-15T08:00:00|-Barclays|Buy|17|
|2016-02-19T08:00:00|2016-02-19T08:00:00|-Barclays|Sell|67.0|
|2016-05-04T08:00:00|2016-05-04T08:00:00|-Barclays|Buy|29|
|2016-06-24T08:00:00|2016-06-24T08:00:00|-Barclays|Buy|28|
|2016-06-27T08:00:00|2016-06-27T08:00:00|-Barclays|Sell|57.00|
|2016-06-27T08:00:00|2016-06-27T08:00:00|stuff-Dunelm|Buy|5|
|2016-06-29T08:00:00|2016-06-29T08:00:00|-Barclays|Buy|27|
|2016-06-30T08:00:00|2016-06-30T08:00:00|-Barclays|Sell|27.000|
|2017-01-13T08:00:00|2017-01-13T08:00:00|stuff-Dunelm|Buy|5|
|2017-07-11T08:00:00|2017-07-11T08:00:00|stuff-Dunelm|Buy|4|
|2017-10-11T08:00:00|2017-10-11T08:00:00|stuff-Dunelm|Sell|14.0|
|2018-05-29T08:00:00|2018-05-29T08:00:00|stuff-Dunelm|Buy|9|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|6|
|2018-09-13T08:00:00|2018-09-13T08:00:00|stuff-Dunelm|Sell|15.00|
|2018-12-19T08:00:00|2018-12-19T08:00:00|stuff-Dunelm|Buy|9|
|2019-01-04T08:00:00|2019-01-04T08:00:00|stuff-Dunelm|Sell|9.000|
|2019-09-05T08:00:00|2019-09-05T08:00:00|stuff-Dunelm|Buy|6|
|2019-09-10T08:00:00|2019-09-10T08:00:00|stuff-Dunelm|Buy|4|
|2019-10-11T08:00:00|2019-10-11T08:00:00|stuff-Dunelm|Buy|4|
|2019-10-14T08:00:00|2019-10-14T08:00:00|stuff-Dunelm|Sell|14.0000|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.023, 0.98,
                new DateTime(2015, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2019, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                20428.2565167236m,
                22,
                15,
                7,
                trades,
                null)
                .SetName("FiveDayStatsRidge-2015-2019-hardbuy");

            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.01, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                17025.5470860290m,
                35,
                28,
                7,
                new Dictionary<DateTime, TradeCollection>(),
                null)
                .SetName("FiveDayStatsLeastSquares-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-01-18T08:00:00|2017-01-18T08:00:00|stuff-Dunelm|Buy|7|
|2017-01-19T08:00:00|2017-01-19T08:00:00|stuff-Dunelm|Buy|5|
|2017-05-24T08:00:00|2017-05-24T08:00:00|stuff-Dunelm|Sell|12.0|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|8|
|2018-07-18T08:00:00|2018-07-18T08:00:00|stuff-Dunelm|Sell|8.00|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLeastSquares,
                null, 1, 1.02, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19275.82m,
                5,
                3,
                2,
                trades,
                null)
                .SetName("FiveDayStatsLeastSquares-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.015, 0.98,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                15066.721629791m,
                46,
                38,
                8,
                new Dictionary<DateTime, TradeCollection>(),
                null)
                .SetName("FiveDayStatsLasso-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2016-07-12T08:00:00|2016-07-12T08:00:00|stuff-Dunelm|Buy|6|
|2016-11-15T08:00:00|2016-11-15T08:00:00|-Barclays|Buy|17|
|2016-12-08T08:00:00|2016-12-08T08:00:00|-Barclays|Buy|12|
|2016-12-09T08:00:00|2016-12-09T08:00:00|-Barclays|Buy|9|
|2017-01-16T08:00:00|2017-01-16T08:00:00|stuff-Dunelm|Sell|6.0|
|2017-09-15T08:00:00|2017-09-15T08:00:00|stuff-Dunelm|Buy|4|
|2018-01-19T08:00:00|2018-01-19T08:00:00|stuff-Dunelm|Sell|4.00|
|2018-07-17T08:00:00|2018-07-17T08:00:00|stuff-Dunelm|Buy|5|
|2018-08-16T08:00:00|2018-08-16T08:00:00|stuff-Dunelm|Sell|5.000|
|2018-09-14T08:00:00|2018-09-14T08:00:00|stuff-Dunelm|Buy|4|
|2018-12-06T08:00:00|2018-12-06T08:00:00|stuff-Dunelm|Buy|3|
|2018-12-07T08:00:00|2018-12-07T08:00:00|stuff-Dunelm|Buy|2|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsLasso,
                null, 1, 1.03, 0.98,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                16041.93514007568320m,
                12,
                9,
                3,
                trades,
                null)
                .SetName("FiveDayStatsLasso-2016-2018-hardbuy");
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.01, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                 17025.54708602m,
                35,
                28,
                7,
                new Dictionary<DateTime, TradeCollection>(),
                null)
                .SetName("FiveDayStatsRidge-2016-2018");
            tradeString =
@$"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2017-01-18T08:00:00|2017-01-18T08:00:00|stuff-Dunelm|Buy|7|
|2017-01-19T08:00:00|2017-01-19T08:00:00|stuff-Dunelm|Buy|5|
|2017-05-24T08:00:00|2017-05-24T08:00:00|stuff-Dunelm|Sell|12.0|
|2018-06-01T08:00:00|2018-06-01T08:00:00|stuff-Dunelm|Buy|8|
|2018-07-18T08:00:00|2018-07-18T08:00:00|stuff-Dunelm|Sell|8.00|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "example-database.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.02, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19275.820000m,
                5,
                3,
                2,
                trades,
                null)
                .SetName("FiveDayStatsRidge-2016-2018-hardbuy");
            tradeString =
$@"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2016-09-13T08:00:00|2016-09-13T08:00:00|Associated British Foods-|Buy|1|
|2016-09-15T08:00:00|2016-09-15T08:00:00|Associated British Foods-|Buy|1|
|2016-09-16T08:00:00|2016-09-16T08:00:00|Associated British Foods-|Buy|1|
|2017-01-13T08:00:00|2017-01-13T08:00:00|Dunelm-|Buy|4|
|2017-01-18T08:00:00|2017-01-18T08:00:00|Dunelm-|Buy|3|
|2017-03-27T08:00:00|2017-03-27T08:00:00|Glencore-|Buy|5|
|2018-02-23T08:00:00|2018-02-23T08:00:00|Dunelm-|Buy|2|
|2018-05-29T08:00:00|2018-05-29T08:00:00|Dunelm-|Buy|1|
|2018-06-01T08:00:00|2018-06-01T08:00:00|Dunelm-|Buy|1|
|2018-07-04T08:00:00|2018-07-04T08:00:00|Glencore-|Buy|2|
|2018-12-07T08:00:00|2018-12-07T08:00:00|Glencore-|Buy|2|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 1, 1.012, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                17162.0211340332m,
                11,
                11,
                0,
                trades,
                null)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy");

            tradeString = @"|StartDate|EndDate|StockName|TradeType|NumberShares|
|-|-|-|-|-|
|2016-09-13T08:00:00|2016-09-13T08:00:00|Associated British Foods-|Buy|1|
|2017-01-16T08:00:00|2017-01-16T08:00:00|Dunelm-|Buy|6|
|2017-09-14T08:00:00|2017-09-14T08:00:00|Dunelm-|Sell|6.0|
|2018-02-21T08:00:00|2018-02-21T08:00:00|Dunelm-|Buy|7|
|2018-02-22T08:00:00|2018-02-22T08:00:00|Dunelm-|Buy|5|
|2018-05-29T08:00:00|2018-05-29T08:00:00|Dunelm-|Buy|4|
|2018-07-04T08:00:00|2018-07-04T08:00:00|Glencore-|Buy|6|
|2018-09-13T08:00:00|2018-09-13T08:00:00|Dunelm-|Sell|16.00|
|2018-12-04T08:00:00|2018-12-04T08:00:00|Glencore-|Sell|6.0|
|2018-12-06T08:00:00|2018-12-06T08:00:00|Glencore-|Buy|15|
";
            trades = new TradeDictionaryBuilder().BuildFromString(tradeString);
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.03, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                19556.20259246826209m,
                10,
                7,
                3,
                trades,
                null)
                .SetName("FiveDayStatsRidge-small-db-2016-2018-hardbuy-5daylater");
            yield return new TestCaseData(
                "small-exchange.xml",
                DecisionSystem.FiveDayStatsRidge,
                null, 5, 1.01, 0.99,
                new DateTime(2016, 1, 4, 15, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 12, 12, 8, 0, 0, DateTimeKind.Utc),
                28210.895393524169m,
                124,
                110,
                14,
                new Dictionary<DateTime, TradeCollection>(),
                null)
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
            Dictionary<DateTime, TradeCollection> expectedTrades,
            double[]? expectedEstimator)
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
            _ = builder.Logging.RegisterLogging(logger);
            _ = builder.Services.RegisterTradingServices(
                new TradingStructures.Common.EvolverSettings(
                    testFilePath,
                    startTime,
                    endTime,
                    TimeSpan.FromDays(1)),
                new(portfolioStartSettings,
                    PortfolioConstructionSettings.Default(),
                    decisionParameters),
                fileSystem);
            var host = builder.Build();
            var output = await host.RunSystemAsync();
            var portfolio = output.Portfolio;
            var trades = output.Trades;

            logger.WriteReportsToFile($"logs\\{DateTime.Now:yyyy-MM-ddTHHmmss}{TestContext.CurrentContext.Test.Name}.log");

            var decisionSystem = host.Services.GetRequiredService<IDecisionSystem>();
            if (decisionSystem is ICalibratedDecisionSystem calibratedDecisionSystem && expectedEstimator != null)
            {
                Assert.That(calibratedDecisionSystem.Result?.Estimator, Is.EquivalentTo(expectedEstimator));
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(20000 - portfolio.TotalValue(Totals.All, startTime.AddDays(-1)), Is.LessThan(tol), "Start value not correct.");
                decimal finalValue = portfolio.TotalValue(Totals.All, endTime);
                Assert.That(Math.Abs(expectedEndValue - finalValue), Is.LessThan(tol), $"End value not correct. Expected {expectedEndValue} but was {finalValue}");
                Assert.That(trades.TotalTrades, Is.EqualTo(expectedNumberTrades), "Number of trades wrong");
                Assert.That(trades.TotalBuyTrades, Is.EqualTo(expectedBuyTrades), "Number of buy trades wrong.");
                Assert.That(trades.TotalSellTrades, Is.EqualTo(expectedSellTrades), "Number of sell trades wrong.");

                string mdTable = trades.ConvertToTable();
                if (expectedTrades.Count > 0)
                {
                    Assert.That(trades.DailyTrades, Is.EquivalentTo(expectedTrades));
                }
            }

        }
    }
}
