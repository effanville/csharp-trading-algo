﻿using System;
using FinancialStructures.Database;
using FinancialStructures.StockStructures;
using Common.Structure.Reporting;
using TradingConsole.DecisionSystem;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.BuySellSystem
{
    /// <summary>
    /// System to enact the buying and selling of stocks.
    /// e.g. one could have a simulation system, or one could use this to interact with a broker.
    /// </summary>
    public interface IBuySellSystem
    {
        /// <summary>
        /// Reports back to the user on errors (and progress)
        /// </summary>
        IReportLogger ReportLogger
        {
            get;
        }

        /// <summary>
        /// Routine to enact all buy and sell decisions.
        /// </summary>
        void BuySell(DateTime day, DecisionStatus status, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);

        /// <summary>
        /// Routine to enact all buy decisions only.
        /// </summary>
        void BuyHolding(DateTime day, Decision buy, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);

        /// <summary>
        /// Routine to enact all sell decisions only.
        /// </summary>
        void SellHolding(DateTime day, Decision buy, IStockExchange stocks, IPortfolio portfolio, TradingStatistics stats, BuySellParams parameters, SimulationParameters simulationParameters);
    }
}