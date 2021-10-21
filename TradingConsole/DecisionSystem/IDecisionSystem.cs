using System;

using TradingConsole.Simulator;
using TradingConsole.DecisionSystem.Models;

namespace TradingConsole.DecisionSystem
{
    /// <summary>
    /// Interface for the system by which one decides at what point to buy and sell stocks.
    /// </summary>
    internal interface IDecisionSystem
    {
        /// <summary>
        /// Sets up various parameters required in the decision system.
        /// e.g. any parameters from an estimation are set at this point.
        /// This may alter the simulation parameters (e.g. the start time of the simulation).
        /// </summary>
        void Calibrate(DecisionSystemSetupSettings decisionParameters, SimulatorSettings settings);

        /// <summary>
        /// The process by which a decision on each stock in the exchange is made.
        /// </summary>
        DecisionStatus Decide(DateTime day, SimulatorSettings settings);
    }
}
