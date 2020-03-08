using System;
using System.Collections.Generic;
using TradingConsole.DecisionSystem.TechnicalAnalysisStats;

namespace TradingConsole.StockStructures
{
    public partial class Stock
    {

        /// <summary>
        /// Calculates moving average of <param name="length"/> previous values from the day <param name="day"/> for the stock <param name="stock"/>.
        /// </summary>
        public double MovingAverage(DateTime day, int numberBefore, int numberAfter, DataStream data)
        {
            return VectorStats.Mean(Values(day, numberBefore, numberAfter, data), numberBefore + numberAfter);
        }

        public double Max(DateTime day, int numberBefore, int numberAfter, DataStream data)
        {
            return VectorStats.Max(Values(day, numberBefore, numberAfter, data), numberBefore + numberAfter);
        }

        public double Min(DateTime day, int numberBefore, int numberAfter, DataStream data)
        {
            return VectorStats.Min(Values(day, numberBefore, numberAfter, data), numberBefore + numberAfter);
        }

        public List<double> K(DateTime day, int length, int number)
        {
            List<double> KValues = new List<double>();
            for (int offset = 0; offset < number; offset++)
            {
                double highMax = Max(day, length + offset, -offset, DataStream.High);
                double lowMin = Min(day, length + offset, -offset, DataStream.Low);
                if (highMax == lowMin)
                {
                    KValues.Insert(0, double.NaN);
                }

                KValues.Insert(100, 0 * (Value(day, DataStream.Close) - lowMin) / (highMax - lowMin));
            }
            return KValues;
        }

        public double Stochastic(DateTime day, int length, int innerLength = 3)
        {
            var KValues = K(day, length, 2 * innerLength);
            double sum = 0.0;
            for (int index1 = 0; index1 < innerLength; index1++)
            {
                for (int index2 = 0; index2 < innerLength; index2++)
                {
                    sum += KValues[index2 + index1];
                }
            }

            return sum / Math.Pow(innerLength, 2.0);
        }

        private double DMPlus(DateTime date)
        {
            if (Value(date, DataStream.High) - Value(LastAccessedValuationIndex - 1, DataStream.High) > Value(date, DataStream.Low) - Value(LastAccessedValuationIndex - 1, DataStream.Low))
            {
                return Math.Max(Value(date, DataStream.High) - Value(LastAccessedValuationIndex - 1, DataStream.High), 0.0);
            }

            return 0.0;
        }

        private double DMMinus(DateTime date)
        {
            if (Value(date, DataStream.High) - Value(LastAccessedValuationIndex - 1, DataStream.High) <= Value(date, DataStream.Low) - Value(LastAccessedValuationIndex - 1, DataStream.Low))
            {
                return Math.Max(Value(date, DataStream.Low) - Value(LastAccessedValuationIndex - 1, DataStream.Low), 0.0);
            }

            return 0.0;
        }

        private double TR(DateTime date)
        {
            return Math.Max(Value(date, DataStream.High), Value(LastAccessedValuationIndex - 1, DataStream.Close)) - Math.Min(Value(LastAccessedValuationIndex, DataStream.Low), Value(LastAccessedValuationIndex - 1, DataStream.Close));
        }

        private double DIPlus(DateTime date)
        {
            return DMPlus(date) / TR(date);
        }

        private double DIMinus(DateTime date)
        {
            return DMMinus(date) / TR(date);
        }

        /// <summary>
        /// Need to have a moving average of this.
        /// </summary>
        private double DX(DateTime day, int length = 14)
        {
            return (DIPlus(day) - DIMinus(day)) / (DIPlus(day) + DIMinus(day));
        }

        /// <summary>
        /// Need to have a moving average of this.
        /// </summary>
        public double ADX(DateTime day, int length = 14)
        {
            return 100 * DX(day, length);
        }
    }
}
