using System;
using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.TradingStructures.Common.Trading;

using Microsoft.Extensions.Logging;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    /// <summary>
    /// Decision system based upon the 5 previous stock days prices.
    /// </summary>
    internal sealed class FiveDayStatsDecisionSystem : ICalibratedDecisionSystem
    {
        private readonly ILogger<FiveDayStatsDecisionSystem> _logger;
        private const int NumberStatistics = 5;
        private readonly DecisionSystemFactory.Settings _settings;
        private Estimator.Result? _estimatorResult;

        public int MinBurnInPeriod => 5 * 25;

        public Estimator.Result? Result => _estimatorResult;

        /// <summary>
        /// Construct and instance.
        /// </summary>
        public FiveDayStatsDecisionSystem(DecisionSystemFactory.Settings settings, ILogger<FiveDayStatsDecisionSystem> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Calibrate(DecisionSystemSettings settings)
        {
            DateTime burnInLength = settings.BurnInEnd;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;

            double[,] fitData = new double[settings.NumberStocks * numberEntries, NumberStatistics];
            double[] fitValues = new double[settings.NumberStocks * numberEntries];
            for (int i = 0; i < numberEntries; i++)
            {
                for (int stockIndex = 0; stockIndex < settings.NumberStocks; stockIndex++)
                {
                    List<double> values = settings.ExchangeHistory.Stocks[stockIndex]
                        .Values(
                            settings.StartTime.AddDays(i),
                            0,
                            NumberStatistics + _settings.DayAfterPredictor,
                            StockDataStream.Open)
                        .Select(Convert.ToDouble)
                        .ToList();

                    double normaliseFactor = values[NumberStatistics - 1];

                    for (int j = 0; j < NumberStatistics; j++)
                    {
                        if (values[j].Equals(double.NaN))
                        {
                            values[j] = values[j + 1];
                        }

                        fitData[i * settings.NumberStocks + stockIndex, j] = values[j] / normaliseFactor;
                    }

                    if (values.Last().Equals(double.NaN))
                    {
                        values[values.Count - 1] = values[values.Count - 2];
                    }

                    fitValues[i * settings.NumberStocks + stockIndex] = values.Last() / normaliseFactor;
                }
            }

            var estimatorType = TypeHelpers.ConvertFrom(_settings.DecisionSystemType);
            if (estimatorType.Success)
            {
                _estimatorResult = Estimator.Fit(estimatorType.Data, fitData, fitValues);
                var values = string.Join(",", _estimatorResult.Estimator);
                _logger.LogWarning($"Estimator Weights are {string.Join(",", _estimatorResult.Estimator)}");
                return;
            }

            _logger.LogInformation($"Created FiveDayStats system without five day stats type.");
        }

        /// <inheritdoc />
        public TradeCollection? Decide(DateTime day, IStockExchange stockExchange)
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
                        NumberStatistics,
                        0,
                        StockDataStream.Open)
                    .Select(Convert.ToDouble)
                    .ToArray();
                double normaliseFactor = values[NumberStatistics - 1];
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

                _logger.LogInformation($"Stock={stock.Name}, Inputs=[{string.Join(",", values)}], Output={value}, Decision={decision}.");

                decisions.Add(stock.Name, decision);
            }

            _logger.LogInformation($"Decisions={decisions}");
            return decisions;
        }
    }
}