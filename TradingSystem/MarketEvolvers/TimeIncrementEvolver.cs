using System;
using System.Collections.Generic;

using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Rates;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Diagnostics;
using Effanville.TradingStructures.Common.Time;
using Effanville.TradingStructures.Common.Trading;
using Effanville.TradingStructures.Pricing;

using TradingSystem.Decisions;
using TradingSystem.PortfolioStrategies;
using TradingSystem.Time;
using TradingSystem.Trading;

namespace TradingSystem.MarketEvolvers
{
    /// <summary>
    /// Provides the logic for simulating the evolution in time
    /// of the prices of a stock market, and gives the functionality
    /// to interact with this stock market by trading at specific points.
    /// <para/>
    /// Further this provides reporting of the trades and decisions taken
    /// at each point in time.
    /// </summary>
    public static class TimeIncrementEvolver
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
        public static EvolverResult Simulate(
            TimeIncrementEvolverSettings simulatorSettings,
            IPriceService priceService,
            IPortfolioManager portfolioManager,
            IDecisionSystem decisionSystem,
            ITradeSubmitter tradeSubmitter,
            Action<string> startReportCallback,
            Action<DateTime, string> reportCallback,
            Action<string> endReportCallback,
            IReportLogger logger)
        {
            TradeHistory decisionRecord = new TradeHistory();
            TradeHistory tradeRecord = new TradeHistory();
            using (new Timer(logger, "Simulation of Evolution"))
            {
                DateTime time = simulatorSettings.BurnInEnd;
                startReportCallback($"StartDate {time} total value {portfolioManager.StartPortfolio.TotalValue(Totals.All):C2}");
                IStockExchange exchange = StockExchangeFactory.Create(simulatorSettings.Exchange, time);
                while (time < simulatorSettings.EndTime)
                {
                    var exchangeOpen = exchange.ExchangeOpenInUtc(time);
                    if (exchangeOpen > time)
                    {
                        time = exchangeOpen;
                    }

                    // update with opening times of the stocks in this time period.
                    StockExchangeFactory.UpdateFromBase(simulatorSettings.Exchange, exchange, time);

                    // ensure that time of evaluation is valid.
                    if (!DateHelpers.IsCalcTimeValid(time, simulatorSettings.CountryDateCode))
                    {
                        time += simulatorSettings.EvolutionIncrement;
                        continue;
                    }

                    // Decide which stocks to buy, sell or do nothing with.
                    TradeCollection decisions = decisionSystem.Decide(time, exchange, logger: null);

                    // Exact the buy/Sell decisions.
                    List<Trade> sellDecisions = decisions.GetSellDecisions();
                    var trades = new TradeCollection(time, time);
                    foreach (Trade sell in sellDecisions)
                    {
                        TradeSubmitterHelpers.SubmitAndReportTrade(
                            time,
                            sell,
                            priceService,
                            portfolioManager,
                            tradeSubmitter,
                            tradeRecord,
                            decisionRecord,
                            logger);
                    }

                    List<Trade> buyDecisions = decisions.GetBuyDecisions();
                    foreach (Trade buy in buyDecisions)
                    {
                        TradeSubmitterHelpers.SubmitAndReportTrade(
                            time,
                            buy,
                            priceService,
                            portfolioManager,
                            tradeSubmitter,
                            tradeRecord,
                            decisionRecord,
                            logger);
                    }

                    // Update the Stock exchange for the recent time period.

                    var exchangeClose = exchange.ExchangeCloseInUtc(time);
                    if (exchangeClose.Date == time.Date && exchangeClose > time)
                    {
                        time = exchangeClose;
                    }
                    StockExchangeFactory.UpdateFromBase(simulatorSettings.Exchange, exchange, time);

                    // update the portfolio values for the new data.
                    portfolioManager.UpdateData(time, exchange);

                    var totalValue = portfolioManager.Portfolio.TotalValue(Totals.All);
                    reportCallback(time, $"Date: {time}. TotalVal: {totalValue:C2}. TotalCash: {portfolioManager.Portfolio.TotalValue(Totals.BankAccount):C2}");

                    time += (simulatorSettings.EvolutionIncrement - time.TimeOfDay);
                }

                endReportCallback($"EndDate {time} total value {portfolioManager.Portfolio.TotalValue(Totals.All):C2}");
                endReportCallback($"EndDate {time} total CAR {portfolioManager.Portfolio.TotalIRR(Totals.All)}");
            }

            return new EvolverResult(portfolioManager.Portfolio, decisionRecord, tradeRecord);
        }
    }
}
