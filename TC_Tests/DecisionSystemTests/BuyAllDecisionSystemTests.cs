using FinancialStructures.StockStructures;
using NUnit.Framework;
using System;
using System.Linq;
using TradingConsole.DecisionSystem;

namespace TC_Tests
{
    public class BuyAllDecisionSystemTests
    {
        [Test]
        public void DecisionAsExpected()
        {
            var exchange = new ExchangeStocks();
            exchange.Stocks.Add(new Stock("MyCompany", "MyName", ""));
            exchange.Stocks[0].AddValue(DateTime.Today, 43, 47, 40, 41, 1);

            var decisionSystem = new BuyAllDecisionSystem(TestHelper.ReportLogger);
            var status = new DecisionStatus();
            decisionSystem.Decide(DateTime.Today, status, exchange, null, null);

            Assert.AreEqual(1, status.GetBuyDecisions().Count);
            Assert.AreEqual(0, status.GetSellDecisions().Count);

            Assert.AreEqual("MyCompany", status.GetBuyDecisionsStockNames().Single());
        }
    }
}
