using System;
using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.Common.Structure.Reporting;

using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Trading;

using TradingSystem.MarketEvolvers;
using TradingSystem.Trading;

namespace TradingSystem.Decisions.Implementation
{
    /// <summary>
    /// Decision system based upon the 5 previous stock days prices.
    /// </summary>
    internal sealed class FiveDayStatsDecisionSystem : IDecisionSystem
    {
        private readonly DecisionSystemFactory.Settings fSettings;
        private Estimator.Result EstimatorResult;

        /// <summary>
        /// Construct and instance.
        /// </summary>
        public FiveDayStatsDecisionSystem(DecisionSystemFactory.Settings settings)
        {
            fSettings = settings;
        }

        /// <inheritdoc />
        public void Calibrate(DecisionSystemSettings settings, IReportLogger logger)
        {
            DateTime burnInLength = settings.BurnInEnd;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;
            int numberStatistics = 5;

            double[,] X = new double[settings.NumberStocks * numberEntries, numberStatistics];
            double[] Y = new double[settings.NumberStocks * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < settings.NumberStocks; stockIndex++)
                {
                    List<double> values = settings.Exchange.Stocks[stockIndex].Values(settings.StartTime.AddDays(i), 0, numberStatistics + fSettings.DayAfterPredictor, StockDataStream.Open).Select(value => Convert.ToDouble(value)).ToList();
                    for (int j = 0; j < numberStatistics; j++)
                    {
                        if (values[j].Equals(double.NaN))
                        {
                            values[j] = values[j + 1];
                        }
                        X[i + stockIndex, j] = values[j] / values[0];
                    }

                    if (values.Last().Equals(double.NaN))
                    {
                        values[values.Count - 1] = values[values.Count - 2];
                    }

                    Y[i + stockIndex] = values.Last() / values[0];
                }
            }

            var estimatorType = TypeHelpers.ConvertFrom(fSettings.DecisionSystemType);
            if (estimatorType.Success)
            {
                EstimatorResult = Estimator.Fit(estimatorType.Data, X, Y);
            }
            else
            {
                _ = logger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown, $"Created FiveDayStats system without five day stats type.");
            }

            _ = logger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Unknown, $"Estimator Weights are {string.Join(",", EstimatorResult.Estimator)}");
        }

        /// <inheritdoc />
        public TradeCollection Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
        {
            var decisions = new TradeCollection(day, day);
            foreach (IStock stock in stockExchange.Stocks)
            {
                TradeType decision = TradeType.Unknown;
                double[] values = stock.Values(day, 5, 0, StockDataStream.Open).Select(value => Convert.ToDouble(value)).ToArray();
                double normaliseFactor = values[0];
                for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
                {
                    values[valueIndex] /= normaliseFactor;
                }

                double value = EstimatorResult.Evaluate(values);

                if (value > fSettings.BuyThreshold)
                {
                    decision = TradeType.Buy;
                }
                else if (value < fSettings.SellThreshold)
                {
                    decision = TradeType.Sell;
                }

                _ = logger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.Execution, $"{stock.Name} - value {value} - decision {decision}.");

                decisions.Add(stock.Name, decision);
            }

            _ = logger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.Execution, $"Decisions: {decisions}");
            return decisions;
        }
    }
}
