using System;

using FinancialStructures.Database;
using FinancialStructures.Database.Extensions.Rates;
using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.StockStructures;

using Nager.Date;

using TradingSystem.Diagnostics;
using TradingSystem.PortfolioStrategies;
using TradingSystem.PriceSystem;
using TradingSystem.Trading;

namespace TradingSystem.Simulator
{
    /// <summary>
    /// Provides the logic for simulating the evolution in time
    /// of the prices of a stock market, and gives the functionality
    /// to interact with this stock market by trading at specific points.
    /// <para/>
    /// Further this provides reporting of the trades and decisions taken
    /// at each point in time.
    /// </summary>
    public static partial class StockMarketEvolver
    {
        /// <summary>
        /// Simulates the evolution of the Stock market from the parameters specified.
        /// Updates an initial portfolio from the start date/burn in date until the 
        /// end date 
        /// </summary>
        /// <param name="simulatorSettings">Contains the exchange and the dates for start and end of the simulation.</param>
        /// <param name="priceService">The method to determine the price to buy or sell at.</param>
        /// <param name="portfolioManager">The portfolio manager which deals with portfolio updates.</param>
        /// <param name="enactTrades">The mechanim by which trades are decided and enacted.</param>
        /// <param name="callbacks">Any reporting callbacks used.</param>
        /// <param name="logger">A logger reporting information.</param>
        /// <returns>A result of the end of the simulation.</returns>
        public static Result Simulate(
            Settings simulatorSettings,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            ITradeEnactor enactTrades,
            Reporting callbacks)
        {
            TradeHistory decisionRecord = new TradeHistory();
            TradeHistory tradeRecord = new TradeHistory();
            using (new Timer(callbacks.Logger, "Simulation of Evolution"))
            {
                DateTime time = simulatorSettings.BurnInEnd;
                callbacks.StartReportCallback($"StartDate {time} total value {portfolioManager.StartPortfolio.TotalValue(Totals.All):C2}");
                IStockExchange exchange = StockExchangeFactory.Create(simulatorSettings.Exchange, time);
                while (time < simulatorSettings.EndTime)
                {
                    // update with opening times of the stocks in this time period.
                    StockExchangeFactory.UpdateFromBase(simulatorSettings.Exchange, exchange, time);

                    // ensure that time of evaluation is valid.
                    if (!IsCalcTimeValid(time, simulatorSettings.CountryDateCode))
                    {
                        time += simulatorSettings.EvolutionIncrement;
                        continue;
                    }

                    // carry out the trades at the desired time.
                    TradeEnactorResult result = enactTrades.EnactTrades(
                        time,
                        exchange,
                        portfolioManager,
                        priceService,
                        callbacks.Logger);

                    // take a record of the decisions and trades.
                    decisionRecord.AddIfNotNull(time, result.Decisions);
                    tradeRecord.AddIfNotNull(time, result.Trades);
                    // Update the Stock exchange for the recent time period.
                    StockExchangeFactory.UpdateFromBase(simulatorSettings.Exchange, exchange, time);

                    // update the portfolio values for the new data.
                    portfolioManager.UpdateData(time, exchange);

                    callbacks.ReportCallback(time, $"Date: {time}. TotalVal: {portfolioManager.Portfolio.TotalValue(Totals.All):C2}. TotalCash: {portfolioManager.Portfolio.TotalValue(Totals.BankAccount):C2}");

                    time += simulatorSettings.EvolutionIncrement;
                }

                callbacks.EndReportCallback($"EndDate {time} total value {portfolioManager.Portfolio.TotalValue(Totals.All):C2}");
                callbacks.EndReportCallback($"EndDate {time} total CAR {portfolioManager.Portfolio.TotalIRR(Totals.All)}");
            }

            return new Result(portfolioManager.Portfolio, decisionRecord, tradeRecord);
        }

        private static bool IsCalcTimeValid(DateTime time, CountryCode countryCode)
        {
            return (time.DayOfWeek != DayOfWeek.Saturday)
                && (time.DayOfWeek != DayOfWeek.Sunday)
                && !DateSystem.IsPublicHoliday(time, countryCode);
        }
    }
}
