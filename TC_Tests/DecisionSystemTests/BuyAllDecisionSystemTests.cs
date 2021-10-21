﻿using System;
using System.Linq;
using FinancialStructures.StockStructures.Implementation;
using NUnit.Framework;
using TradingConsole.DecisionSystem.Implementation;
using TradingConsole.DecisionSystem.Models;
using TradingConsole.Simulator;

namespace TC_Tests
{
    public class BuyAllDecisionSystemTests
    {
        [Test]
        public void DecisionAsExpected()
        {
            var exchange = new StockExchange();
            exchange.Stocks.Add(new Stock("MyTicker", "MyCompany", "MyName", ""));
            exchange.Stocks[0].AddValue(DateTime.Today, 43, 47, 40, 41, 1);

            BuyAllDecisionSystem decisionSystem = new BuyAllDecisionSystem();
            var settings = new SimulatorSettings(default, default, default, exchange);
            DecisionStatus status = decisionSystem.Decide(DateTime.Today, settings);

            Assert.AreEqual(1, status.GetBuyDecisions().Count);
            Assert.AreEqual(0, status.GetSellDecisions().Count);

            Assert.AreEqual("MyCompany", status.GetBuyDecisionsStockNames().Single());
        }
    }
}
