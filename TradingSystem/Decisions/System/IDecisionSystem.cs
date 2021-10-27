﻿using System;

using Common.Structure.Reporting;

using FinancialStructures.StockStructures;

using TradingSystem.Decisions.Models;
using TradingSystem.Simulator;

namespace TradingSystem.Decisions.System
{
    /// <summary>
    /// Interface for the system by which one decides at what point to buy and sell stocks.
    /// </summary>
    public interface IDecisionSystem
    {
        /// <summary>
        /// Sets up various parameters required in the decision system.
        /// e.g. any parameters from an estimation are set at this point.
        /// This may alter the simulation parameters (e.g. the start time of the simulation).
        /// </summary>
        void Calibrate(SimulatorSettings settings, IReportLogger logger);

        /// <summary>
        /// The process by which a decision on each stock in the exchange is made.
        /// </summary>
        DecisionStatus Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger);
    }
}