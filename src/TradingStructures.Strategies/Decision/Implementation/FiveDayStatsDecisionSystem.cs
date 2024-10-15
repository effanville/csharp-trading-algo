using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    /// <summary>
    /// Decision system based upon the 5 previous stock days prices.
    /// </summary>
    internal sealed class FiveDayStatsDecisionSystem : IDecisionSystem
    {
        private const int NumberStatistics = 5;
        private readonly DecisionSystemSetupSettings _settings;
        private Estimator.Result? _estimatorResult;

        /// <summary>
        /// Construct and instance.
        /// </summary>
        public FiveDayStatsDecisionSystem(DecisionSystemSetupSettings settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public void Calibrate(DecisionSystemSettings settings, IReportLogger? logger)
        {
            DateTime burnInLength = settings.BurnInEnd;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;

            double[,] fitData = new double[settings.NumberStocks * numberEntries, NumberStatistics];
            double[] fitValues = new double[settings.NumberStocks * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < settings.NumberStocks; stockIndex++)
                {
                    List<double> values = settings.Exchange.Stocks[stockIndex]
                        .Values(
                            settings.StartTime.AddDays(i), 
                            0, 
                            NumberStatistics + _settings.DayAfterPredictor,
                            StockDataStream.Open)
                        .Select(Convert.ToDouble)
                        .ToList();
                    double normalisationConstant = 0.0;
                    
                    for (int j = 0; j < NumberStatistics; j++)
                    {
                        normalisationConstant += values[j];
                    }

                    normalisationConstant /= NumberStatistics;
                    for (int j = 0; j < NumberStatistics; j++)
                    {
                        if (values[j].Equals(double.NaN))
                        {
                            values[j] = values[j + 1];
                        }

                        fitData[i * settings.NumberStocks + stockIndex, j] = values[j] / normalisationConstant;
                    }

                    if (values.Last().Equals(double.NaN))
                    {
                        values[values.Count - 1] = values[values.Count - 2];
                    }

                    fitValues[i * settings.NumberStocks + stockIndex] = values.Last() / normalisationConstant;
                }
            }

            var estimatorType = TypeHelpers.ConvertFrom(_settings.DecisionSystemType);
            if (estimatorType.Success)
            {
                _estimatorResult = Estimator.Fit(estimatorType.Data, fitData, fitValues);
                _ = logger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown,
                    $"FitData....... FitValue");
                for (int i = 0; i < fitData.GetLength(0); i++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < fitData.GetLength(1); j++)
                    {
                        sb.Append(fitData[i, j] + ",");
                    }

                    sb.Append("  " + fitValues[i]);
                    _ = logger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown,
                        sb.ToString());
                }
                
                _ = logger?.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Unknown,
                    $"Estimator Weights are {string.Join(",", _estimatorResult.Estimator)}");
                return;
            }

            _ = logger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown,
                $"Created FiveDayStats system without five day stats type.");
        }

        /// <inheritdoc />
        public TradeCollection? Decide(DateTime day, IStockExchange stockExchange, IReportLogger? logger)
        {
            if (_estimatorResult == null)
            {
                return null;
            }

            var decisions = new TradeCollection(day, day);
            foreach (IStock stock in stockExchange.Stocks)
            {
                TradeType decision = TradeType.Unknown;
                double[] values = stock.Values(
                        day, 
                        5, 
                        0,
                        StockDataStream.Open)
                    .Select(Convert.ToDouble)
                    .ToArray();
                double normaliseFactor = values[0];
                for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
                {
                    values[valueIndex] /= normaliseFactor;
                }

                double value = _estimatorResult.Evaluate(values);

                if (value > _settings.BuyThreshold)
                {
                    decision = TradeType.Buy;
                }
                else if (value < _settings.SellThreshold)
                {
                    decision = TradeType.Sell;
                }

                _ = logger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.Execution,
                    $"Stock={stock.Name}, Inputs=[{string.Join(",",values)}], Output={value}, Decision={decision}.");

                decisions.Add(stock.Name, decision);
            }

            _ = logger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.Execution,
                $"Decisions={decisions}");
            return decisions;
        }
    }
}