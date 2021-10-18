﻿using System;
using System.IO.Abstractions;
using FinancialStructures.Database;
using FinancialStructures.StockStructures;
using Common.Structure.DataStructures;
using Common.Structure.Reporting;
using TradingConsole.BuySellSystem;
using TradingConsole.DecisionSystem;
using FinancialStructures.NamingStructures;
using TradingConsole.DecisionSystem.Models;

namespace TradingConsole.Simulator
{
    internal partial class TradingSimulation
    {
        private readonly IDecisionSystem DecisionSystem;
        private readonly ITradeMechanism BuySellSystem;
        private readonly TradeMechanismSettings fTradeMechanismSettings;
        private readonly TradeMechanismTraderOptions fTraderOptions;
        private readonly SimulatorSettings fSimulatorSettings;
        private readonly IPortfolio fPortfolio;

        private readonly IReportLogger ReportLogger;
        private readonly IFileSystem fFileSystem;

        /// <summary>
        /// Does the setup of the simulation error.
        /// </summary>
        public bool SetupError
        {
            get;
            set;
        }

        public TradingSimulation(
            string stockFilePath,
            DateTime startTime,
            DateTime endTime,
            TimeSpan evolutionIncrement,
            PortfolioStartSettings startSettings,
            DecisionSystemSetupSettings decisionParameters,
            TradeMechanismType buySellType,
            IFileSystem fileSystem,
            IReportLogger reportLogger)
        {
            ReportLogger = reportLogger;
            fFileSystem = fileSystem;

            fTradeMechanismSettings = new TradeMechanismSettings();
            fTraderOptions = new TradeMechanismTraderOptions();

            IStockExchange exchange;
            using (new Timer(ReportLogger, "Setup"))
            {
                using (new Timer(ReportLogger, "Loading Exchange"))
                {
                    exchange = StockExchangeFactory.Create(stockFilePath, fFileSystem, ReportLogger);
                    if (!exchange.CheckValidity())
                    {
                        _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Stock input data not suitable.");
                        SetupError = true;
                        return;
                    }

                    fSimulatorSettings = new SimulatorSettings(
                        startTime,
                        endTime,
                        evolutionIncrement,
                        exchange);
                }

                using (new Timer(reportLogger, "Calibrating"))
                {
                    DecisionSystem = DecisionSystemFactory.Create(decisionParameters.DecisionSystemType, ReportLogger);
                    DecisionSystem.Calibrate(decisionParameters, fSimulatorSettings);
                }

                BuySellSystem = TradeMechanismFactory.Create(buySellType, ReportLogger);

                using (new Timer(reportLogger, "Loading Portfolio"))
                {
                    fPortfolio = LoadStartPortfolio(startSettings, reportLogger);
                }
            }
        }

        public (DecisionRecord, IPortfolio) SimulateRun()
        {
            var record = new DecisionRecord();
            var portfolio = fPortfolio.Copy();

            bool isCalcTimeValid(DateTime time) => (time.DayOfWeek != DayOfWeek.Saturday) || (time.DayOfWeek != DayOfWeek.Sunday);
            void reportCallback(DateTime time, string message)
            {
                if (time.Day == 1)
                {
                    _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.DatabaseAccess, message);
                }
            }

            void startEndReportCallback(string message)
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.DatabaseAccess, message);

            }

            TradeSimulator.Simulate(fSimulatorSettings,
                isCalcTimeValid,
                startEndReportCallback,
                reportCallback,
                startEndReportCallback,
                portfolio,
                (time, exc, port) => PerformDailyTrades(time, exc, port, record),
                (time, exc, port) => UpdatePortfolioData(time, exc, port),
                ReportLogger);
            return (record, portfolio);
        }

        private void PerformDailyTrades(DateTime day, IStockExchange exchange, IPortfolio portfolio, DecisionRecord record)
        {
            // Decide which stocks to buy, sell or do nothing with.
            DecisionStatus status = DecisionSystem.Decide(day, fSimulatorSettings, record);

            double CalculatePurchasePrice(DateTime time, NameData stock)
            {
                double openPrice = exchange.GetValue(stock, time, StockDataStream.Open);

                // we modify the price we buy at from the opening price, to simulate market movement.
                double upDown = fTradeMechanismSettings.RandomNumbers.Next(0, 100) > 100 * fTradeMechanismSettings.UpTickProbability ? 1 : -1;
                double valueModifier = 1 + fTradeMechanismSettings.UpTickSize * upDown;
                return openPrice * valueModifier;
            }

            double CalculateSellPrice(DateTime time, NameData stock)
            {
                // First calculate price that one sells at.
                // This is the open price of the stock, with a combat multiplier.
                double upDown = fTradeMechanismSettings.RandomNumbers.Next(0, 100) > 100 * fTradeMechanismSettings.UpTickProbability ? 1 : -1;
                double valueModifier = 1 + fTradeMechanismSettings.UpTickSize * upDown;
                return exchange.GetValue(stock, time, StockDataStream.Open) * valueModifier;
            }

            // Exact the buy/Sell decisions.
            BuySellSystem.EnactAllTrades(
                day,
                status,
                (date, name) => CalculatePurchasePrice(date, name),
                (date, name) => CalculateSellPrice(date, name),
                portfolio,
                fTraderOptions);
        }

        private IPortfolio LoadStartPortfolio(PortfolioStartSettings settings, IReportLogger logger)
        {
            IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
            if (settings.PortfolioFilePath != null)
            {
                portfolio.LoadPortfolio(settings.PortfolioFilePath, fFileSystem, ReportLogger);
            }
            else
            {
                _ = portfolio.TryAdd(Account.BankAccount, new NameData(settings.DefaultBankAccName.Company, settings.DefaultBankAccName.Name), logger);
                var data = new DailyValuation(settings.StartTime.AddDays(-1), settings.StartingCash);
                _ = portfolio.TryAddOrEditData(Account.BankAccount, settings.DefaultBankAccName, data, data, logger);
            }

            return portfolio;
        }

        private static void UpdatePortfolioData(DateTime day, IStockExchange stocks, IPortfolio portfolio)
        {
            foreach (var security in portfolio.FundsThreadSafe)
            {
                if (security.Value(day).Value > 0)
                {
                    double value = stocks.GetValue(new TwoName(security.Names.Company, security.Names.Name), day);
                    if (!value.Equals(double.NaN))
                    {
                        security.SetData(day, value);
                    }
                }
            }
        }
    }
}
