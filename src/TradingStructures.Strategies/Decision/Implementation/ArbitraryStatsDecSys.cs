using System;
using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Structure.MathLibrary.ParameterEstimation;
using Effanville.Common.Structure.Reporting;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.Stocks;
using Effanville.FinancialStructures.Stocks.Statistics;
using Effanville.TradingStructures.Common.Trading;

namespace Effanville.TradingStructures.Strategies.Decision.Implementation
{
    /// <summary>
    /// A decision system consisting of arbitrary statistics. It has a least
    /// squares regression estimator to obtain the best fit to these
    /// statistics.
    /// </summary>
    internal sealed class ArbitraryStatsDecisionSystem : IDecisionSystem
    {
        private readonly DecisionSystemFactory.Settings _settings;
        private readonly IReadOnlyList<IStockStatistic> _stockStatistics;
        private Estimator.Result? _estimatorResult;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public ArbitraryStatsDecisionSystem(DecisionSystemFactory.Settings decisionParameters)
        {
            List<IStockStatistic> stockStatistics = new List<IStockStatistic>();
            if (decisionParameters.Statistics != null)
            {
                stockStatistics.AddRange(decisionParameters.Statistics.Select(StockStatisticFactory.Create));
            }

            _settings = decisionParameters;
            _stockStatistics = stockStatistics;
        }

        /// <inheritdoc/>
        public void Calibrate(DecisionSystemSettings settings, IReportLogger logger)
        {
            DateTime burnInLength = settings.BurnInEnd;

            int delayTime = _stockStatistics.Max(stock => stock.BurnInTime) + 2;
            int numberEntries = ((burnInLength - settings.StartTime).Days - 5) * 5 / 7;
            int numberStatistics = _stockStatistics.Count;

            double[,] fitData = new double[settings.NumberStocks * numberEntries, numberStatistics];
            double[] fitValues = new double[settings.NumberStocks * numberEntries];
            for (int entryIndex = 0; entryIndex < numberEntries; entryIndex++)
            {
                for (int stockIndex = 0; stockIndex < settings.NumberStocks; stockIndex++)
                {
                    for (int statisticIndex = 0; statisticIndex < numberStatistics; statisticIndex++)
                    {
                        fitData[entryIndex * settings.Exchange.Stocks.Count + stockIndex, statisticIndex] =
                            _stockStatistics[statisticIndex]
                                .Calculate(
                                    settings.StartTime.AddDays(delayTime + entryIndex),
                                    settings.Exchange.Stocks[stockIndex]);
                    }

                    fitValues[entryIndex * settings.Exchange.Stocks.Count + stockIndex] = Convert.ToDouble(
                        settings.Exchange.Stocks[stockIndex].Values(burnInLength.AddDays(delayTime + entryIndex), 0,
                            _settings.DayAfterPredictor, StockDataStream.Open).Last() / 100m);
                }
            }

            var estimatorType = TypeHelpers.ConvertFrom(_settings.DecisionSystemType);
            if (!estimatorType.Success)
            {
                _estimatorResult = Estimator.Fit(estimatorType.Data, fitData, fitValues);
                _ = logger.Log(ReportSeverity.Critical, ReportType.Warning, ReportLocation.Unknown,
                    $"Estimator Weights are {string.Join(",", _estimatorResult.Estimator)}");
                return;
            }

            _ = logger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Unknown,
                $"Created ArbitraryStats system without correct type.");
        }

        /// <inheritdoc/>
        public TradeCollection? Decide(DateTime day, IStockExchange stockExchange, IReportLogger logger)
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
                double value = _estimatorResult.Evaluate(values);

                if (value > _settings.BuyThreshold)
                {
                    decision = TradeType.Buy;
                }
                else if (value < _settings.SellThreshold)
                {
                    decision = TradeType.Sell;
                }

                decisions.Add(stock.Name, decision);
            }

            return decisions;
        }
    }
}