﻿using System;
using System.Collections.Generic;
using FinancialStructures.StockStructures;
using StructureCommon.Reporting;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    /// <summary>
    /// Decision system which at any time reports to buy every stock held in the exchange.
    /// </summary>
    public class BuyAllDecisionSystem : IDecisionSystem
    {
        /// <inheritdoc cref="IDecisionSystem"/>
        public IReportLogger ReportLogger
        {
            get;
        }

        public BuyAllDecisionSystem(IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
        }

        /// <summary>
        /// No parameters to calibrate in this system.
        /// </summary>
        public void Calibrate(UserInputOptions inputOptions, ExchangeStocks exchange, SimulationParameters simulationParameters)
        {
        }

        /// <inheritdoc cref="IDecisionSystem"/>
        public void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells)
        {
            stats.AddDailyDecisionStats(day, buys, sells);
        }

        /// <inheritdoc cref="IDecisionSystem"/>
        public void Decide(DateTime date, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats, SimulationParameters simulationParameters)
        {
            foreach (Stock stock in exchange.Stocks)
            {
                status.AddDecision(stock.Name, StockTradeDecision.Buy);
            }
        }
    }
}
