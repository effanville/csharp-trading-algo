using System;
using Common.Structure.Reporting;
using FinancialStructures.Database;
using FinancialStructures.StockStructures;

namespace TradingConsole.Simulator
{
    internal static class TradeSimulator
    {
        public static void Simulate(
            DateTime startTime,
            DateTime endTime,
            TimeSpan evolutionIncrement,
            Func<DateTime, bool> isCalcTimeValid,
            Action<string> startReportCallback,
            Action<DateTime, string> reportCallback,
            Action<string> endReportCallback,
            IStockExchange exchange,
            IPortfolio portfolio,
            Action<DateTime, IStockExchange, IPortfolio> enactTrades,
            Action<DateTime, IStockExchange, IPortfolio> portfolioUpdate,
            IReportLogger logger)
        {
            Simulate(
                new SimulatorSettings(startTime, endTime, evolutionIncrement, exchange),
                isCalcTimeValid,
                startReportCallback,
                reportCallback,
                endReportCallback,
                portfolio,
                enactTrades,
                portfolioUpdate,
                logger);
        }

        public static void Simulate(
            SimulatorSettings settings,
            Func<DateTime, bool> isCalcTimeValid,
            Action<string> startReportCallback,
            Action<DateTime, string> reportCallback,
            Action<string> endReportCallback,
            IPortfolio portfolio,
            Action<DateTime, IStockExchange, IPortfolio> enactTrades,
            Action<DateTime, IStockExchange, IPortfolio> portfolioUpdate,
            IReportLogger logger
    )
        {
            using (new Timer(logger, "Simulation"))
            {
                DateTime time = settings.StartTime;
                startReportCallback($"StartDate {time} total value {portfolio.TotalValue(Totals.All):C2}");
                while (time < settings.EndTime)
                {
                    // ensure that time of evaluation is valid.
                    if (!isCalcTimeValid(time))
                    {
                        time += settings.EvolutionIncrement;
                        continue;
                    }

                    // carry out the trades at the desired time.
                    enactTrades(time, settings.Exchange, portfolio);

                    // update the portfolio values for the new data.
                    portfolioUpdate(time, settings.Exchange, portfolio);

                    reportCallback(time, $"Date {time} total value {portfolio.TotalValue(Totals.All):C2}");

                    time += settings.EvolutionIncrement;
                }

                endReportCallback($"EndDate {time} total value {portfolio.TotalValue(Totals.All):C2}");
            }
        }
    }
}
