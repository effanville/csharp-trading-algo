using FinancialStructures.ReportLogging;
using FinancialStructures.StockStructures;
using System;
using System.Collections.Generic;
using TradingConsole.Simulation;
using TradingConsole.Statistics;

namespace TradingConsole.DecisionSystem
{
    public class BasicDecisionSystem : IDecisionSystem
    {
        public LogReporter ReportLogger { get; }

        public BasicDecisionSystem(LogReporter reportLogger)
        {
            ReportLogger = reportLogger;
        }

        public void AddDailyDecisionStats(TradingStatistics stats, DateTime day, List<string> buys, List<string> sells)
        {
            stats.AddDailyDecisionStats(day, buys, sells);
        }

        public void Decide(DateTime date, DecisionStatus status, ExchangeStocks exchange, TradingStatistics stats, SimulationParameters simulationParameters)
        {
            foreach (var stock in exchange.Stocks)
            {
                StockTradeDecision decision;
                var values = stock.Values(date, 0, 5, DataStream.Open).ToArray();
                double value = simulationParameters.Estimator.Evaluate(values);

                if (value > 1.05)
                {
                    decision = StockTradeDecision.Buy;
                }
                else if (value < 1)
                {
                    decision = StockTradeDecision.Sell;
                }
                else
                {
                    decision = StockTradeDecision.Hold;
                }

                status.AddDecision(stock.Name, decision);
            }
        }
    }
}
