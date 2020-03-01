using System;
using TradingConsole.StockStructures;

namespace TradingConsole.DecisionSystem.TechnicalAnalysisStats
{
    public static class Statistics
    {
        /// <summary>
        /// Calculates moving average of <param name="length"/> previous values from the day <param name="day"/> for the stock <param name="stock"/>.
        /// </summary>
        public static double MovingAverage(Stock stock, DateTime day, int length)
        {
            double sum = 0.0;
            for (int index = 0; index < length; index++)
            {
                sum += stock.Value(day);
            }

            sum /= length;
            return sum;
        }
    }
}
