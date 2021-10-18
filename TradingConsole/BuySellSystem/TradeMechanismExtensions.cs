using System;
using FinancialStructures.Database;
using TradingConsole.DecisionSystem.Models;
using System.Collections.Generic;
using FinancialStructures.NamingStructures;

namespace TradingConsole.BuySellSystem
{
    public static class TradeMechanismExtensions
    {
        public static void EnactAllTrades(
            this ITradeMechanism tradeMechanism,
            DateTime time,
            DecisionStatus decisions,
            Func<DateTime, NameData, double> calculateBuyPrice,
            Func<DateTime, NameData, double> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions)
        {
            List<Decision> sellDecisions = decisions.GetSellDecisions();
            foreach (Decision sell in sellDecisions)
            {
                _ = tradeMechanism.Sell(time, sell, calculateSellPrice, portfolio, traderOptions);
            }

            List<Decision> buyDecisions = decisions.GetBuyDecisions();
            foreach (Decision buy in buyDecisions)
            {
                _ = tradeMechanism.Buy(time, buy, calculateBuyPrice, portfolio, traderOptions);
            }
        }
    }
}
