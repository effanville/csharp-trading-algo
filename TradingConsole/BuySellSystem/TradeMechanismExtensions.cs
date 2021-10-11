using System;
using FinancialStructures.Database;
using FinancialStructures.StockStructures;
using TradingConsole.DecisionSystem.Models;
using System.Collections.Generic;

namespace TradingConsole.BuySellSystem
{
    public static class TradeMechanismExtensions
    {
        public static void EnactAllTrades(
            this ITradeMechanism tradeMechanism,
            DateTime time,
            DecisionStatus decisions,
            IStockExchange exchange,
            IPortfolio portfolio,
            TradeMechanismSettings settings,
            TradeMechanismTraderOptions traderOptions)
        {
            List<Decision> sellDecisions = decisions.GetSellDecisions();

            foreach (Decision sell in sellDecisions)
            {
                _ = tradeMechanism.Sell(time, sell, exchange, portfolio, settings, traderOptions);
            }

            List<Decision> buyDecisions = decisions.GetBuyDecisions();

            foreach (Decision buy in buyDecisions)
            {
                _ = tradeMechanism.Buy(time, buy, exchange, portfolio, settings, traderOptions);
            }
        }
    }
}
