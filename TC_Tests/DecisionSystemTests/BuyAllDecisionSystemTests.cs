using System;
using System.Linq;

using FinancialStructures.StockStructures.Implementation;

using NUnit.Framework;

using TradingConsole.DecisionSystem.Implementation;

using TradingSystem.DecideThenTradeSystem;
using TradingSystem.Simulator.Trading.Decisions;

namespace TradingConsole.Tests.DecisionSystemTests
{
    internal class BuyAllDecisionSystemTests
    {
        [Test]
        public void DecisionAsExpected()
        {
            var exchange = new StockExchange();
            exchange.Stocks.Add(new Stock("MyTicker", "MyCompany", "MyName", "GBP", ""));
            exchange.Stocks[0].AddValue(DateTime.Today, 43, 47, 40, 41, 1);

            IDecisionSystem decisionSystem = new BuyAllDecisionSystem();
            DecisionStatus status = decisionSystem.Decide(DateTime.Today, exchange, logger: null);

            Assert.AreEqual(1, status.GetBuyDecisions().Count);
            Assert.AreEqual(0, status.GetSellDecisions().Count);

            var name = status.GetBuyDecisions().Single().StockName;
            Assert.AreEqual("MyCompany-MyName", name.ToString());
        }
    }
}
