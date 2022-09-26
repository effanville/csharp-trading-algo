using System;
using System.Collections.Generic;

using Common.Structure.Reporting;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using TradingSystem.Simulator.Trading.Decisions;
using TradingSystem.Simulator.Trading;

namespace TradingSystem.DecideThenTradeSystem
{
    public static class TradeMechanismExtensions
    {
        public static TradeStatus SellThenBuy(
            this ITradeMechanism tradeMechanism,
            DateTime time,
            DecisionStatus decisions,
            Func<DateTime, TwoName, decimal> calculateBuyPrice,
            Func<DateTime, TwoName, decimal> calculateSellPrice,
            IPortfolio portfolio,
            TradeMechanismTraderOptions traderOptions,
            IReportLogger reportLogger)
        {
            List<Decision> sellDecisions = decisions.GetSellDecisions();
            int numberSells = 0;
            foreach (Decision sell in sellDecisions)
            {
                if (tradeMechanism.Sell(time, sell, calculateSellPrice, portfolio, traderOptions, reportLogger))
                {
                    numberSells++;
                }
            }

            int numberBuys = 0;
            List<Decision> buyDecisions = decisions.GetBuyDecisions();
            foreach (Decision buy in buyDecisions)
            {
                if (tradeMechanism.Buy(time, buy, calculateBuyPrice, portfolio, traderOptions, reportLogger))
                {
                    numberBuys++;
                }
            }

            return new TradeStatus(numberBuys, numberSells);
        }
    }
}
