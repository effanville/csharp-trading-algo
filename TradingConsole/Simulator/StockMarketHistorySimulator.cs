using System;
using Common.Structure.Reporting;
using FinancialStructures.Database;
using FinancialStructures.Database.Statistics;
using FinancialStructures.NamingStructures;
using FinancialStructures.StockStructures;
using TradingConsole.BuySellSystem;
using TradingConsole.BuySellSystem.Models;
using TradingConsole.DecisionSystem.Models;

namespace TradingConsole.Simulator
{
    /// <summary>
    /// Provides the logic for simulating the evolution in time
    /// of the prices of a stock market, and gives the functionality
    /// to interact with this stock market by trading at specific points.
    /// <para/>
    /// Further this provides reporting of the trades and decisions taken
    /// at each point in time.
    /// </summary>
    internal static class StockMarketHistorySimulator
    {
        public static (IPortfolio, DecisionRecord, TradeRecord) Simulate(
            DateTime startTime,
            DateTime endTime,
            TimeSpan evolutionIncrement,
            TradeMechanismSettings purchaseSettings,
            Func<DateTime, bool> isCalcTimeValid,
            Action<string> startReportCallback,
            Action<DateTime, string> reportCallback,
            Action<string> endReportCallback,
            IStockExchange exchange,
            IPortfolio portfolio,
            Func<DateTime, IPortfolio, Func<DateTime, TwoName, double>, Func<DateTime, TwoName, double>, (TradeStatus, DecisionStatus)> enactTrades,
            IReportLogger logger)
        {
            return Simulate(
                new SimulatorSettings(startTime, endTime, evolutionIncrement, exchange),
                purchaseSettings,
                isCalcTimeValid,
                startReportCallback,
                reportCallback,
                endReportCallback,
                portfolio,
                enactTrades,
                logger);
        }

        public static (IPortfolio, DecisionRecord, TradeRecord) Simulate(
            SimulatorSettings settings,
            TradeMechanismSettings purchaseSettings,
            Func<DateTime, bool> isCalcTimeValid,
            Action<string> startReportCallback,
            Action<DateTime, string> reportCallback,
            Action<string> endReportCallback,
            IPortfolio portfolio,
            Func<DateTime, IPortfolio, Func<DateTime, TwoName, double>, Func<DateTime, TwoName, double>, (TradeStatus, DecisionStatus)> enactTrades,
            IReportLogger logger)
        {
            DecisionRecord decisionRecord = new DecisionRecord();
            TradeRecord tradeRecord = new TradeRecord();
            using (new Timer(logger, "Simulation"))
            {
                DateTime time = settings.StartTime;
                startReportCallback($"StartDate {time} total value {portfolio.TotalValue(Totals.All):C2}");
                IStockExchange exchange = StockExchangeFactory.Create(settings.Exchange, time);
                while (time < settings.EndTime)
                {
                    // update with opening times of the stocks in this time period.
                    StockExchangeFactory.UpdateFromBase(settings.Exchange, exchange, time, openOnly: true);

                    // ensure that time of evaluation is valid.
                    if (!isCalcTimeValid(time))
                    {
                        time += settings.EvolutionIncrement;
                        continue;
                    }

                    // carry out the trades at the desired time.
                    var (trades, decisions) = enactTrades(
                        time,
                        portfolio,
                        (time, name) => CalculatePurchasePrice(time, purchaseSettings, exchange, name),
                        (time, name) => CalculateSellPrice(time, purchaseSettings, exchange, name));

                    // take a record of the decisions and trades.
                    decisionRecord.AddForTheRecord(time, decisions);
                    tradeRecord.AddForTheRecord(time, trades);

                    // Update the Stock exchange for the recent time period.
                    StockExchangeFactory.UpdateFromBase(settings.Exchange, exchange, time, openOnly: false);

                    // update the portfolio values for the new data.
                    UpdatePortfolioData(time, exchange, portfolio);

                    reportCallback(time, $"Date {time} total value {portfolio.TotalValue(Totals.All):C2}");

                    time += settings.EvolutionIncrement;
                }

                endReportCallback($"EndDate {time} total value {portfolio.TotalValue(Totals.All):C2}");
                endReportCallback($"EndDate {time} total CAR {portfolio.TotalIRR(Totals.All)}");
            }

            return (portfolio, decisionRecord, tradeRecord);
        }

        private static double CalculatePurchasePrice(DateTime time, TradeMechanismSettings purchaseSettings, IStockExchange exchange, TwoName stock)
        {
            double openPrice = exchange.GetValue(stock, time, StockDataStream.Open);

            // we modify the price we buy at from the opening price, to simulate market movement.
            double upDown = purchaseSettings.RandomNumbers.Next(0, 100) > 100 * purchaseSettings.UpTickProbability ? 1 : -1;
            double valueModifier = 1 + purchaseSettings.UpTickSize * upDown;
            return openPrice * valueModifier;
        }

        private static double CalculateSellPrice(DateTime time, TradeMechanismSettings purchaseSettings, IStockExchange exchange, TwoName stock)
        {
            // First calculate price that one sells at.
            // This is the open price of the stock, with a combat multiplier.
            double upDown = purchaseSettings.RandomNumbers.Next(0, 100) > 100 * purchaseSettings.UpTickProbability ? 1 : -1;
            double valueModifier = 1 + purchaseSettings.UpTickSize * upDown;
            return exchange.GetValue(stock, time, StockDataStream.Open) * valueModifier;
        }

        private static void UpdatePortfolioData(DateTime day, IStockExchange exchange, IPortfolio portfolio)
        {
            foreach (var security in portfolio.FundsThreadSafe)
            {
                if (security.Value(day).Value > 0)
                {
                    double value = exchange.GetValue(security.Names.ToTwoName(), day);
                    if (!value.Equals(double.NaN))
                    {
                        security.SetData(day, value);
                    }
                }
            }
        }
    }
}
