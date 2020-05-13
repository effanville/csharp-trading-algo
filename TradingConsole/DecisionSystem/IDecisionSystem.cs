using StructureCommon.Reporting;
using FinancialStructures.StockStructures;
using System;
using System.Collections.Generic;
using TradingConsole.InputParser;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    /// <summary>
    /// Interface for the system by which one decides at what point to buy and sell stocks.
    /// </summary>
    interface IDecisionSystem
    {
        /// <summary>
        /// Reports back to the user on errors (and progress)
        /// </summary>
        LogReporter ReportLogger { get; }

        /// <summary>
        /// Sets up various parameters required in the decision system.
        /// e.g. any parameters from an estimation are set at this point.
        /// This may alter the simulation parameters (e.g. the start time of the simulation).
        /// </summary>
        void Calibrate(UserInputOptions inputOptions, ExchangeStocks exchange, SimulationParameters simulationParameters);

        /// <summary>
        /// The process by which a decision on each stock in the exchange is made.
        /// </summary>
        void Decide(DateTime day, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats, SimulationParameters simulationParameters);

        /// <summary>
        /// Method to add to statistics information about what decisions have been made.
        /// </summary>
        void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells);
    }
}
