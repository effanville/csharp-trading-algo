using System;
using System.Collections.Generic;

namespace TradingConsole.DecisionSystem.TechnicalAnalysisStats
{
    public static class VectorStats
    {
        public static double Max(List<double> values, int number)
        {
            if (values.Count < number)
            {
                return double.NaN;
            }
            double maximum = 0.0;
            for (int index = 0; index < number; index++)
            {
                double latestVal = values[values.Count - 1 - index];
                if (maximum < latestVal)
                {
                    maximum = latestVal;
                }
            }

            return maximum;
        }

        public static double Min(List<double> values, int number)
        {
            if (values.Count < number)
            {
                return double.NaN;
            }
            double minimum = 0.0;
            for (int index = 0; index < number; index++)
            {
                double latestVal = values[values.Count - 1 - index];
                if (minimum > latestVal)
                {
                    minimum = latestVal;
                }
            }

            return minimum;
        }

        public static double Mean(List<double> values, int number)
        {
            if (values.Count < number)
            {
                return double.NaN;
            }
            double sum = 0.0;
            for (int index = 0; index < number; index++)
            {
                sum += values[values.Count - 1 - index];
            }

            return sum / number;
        }

        public static double Variance(List<double> values, int number)
        {
            if (values.Count < number || number.Equals(1.0))
            {
                return double.NaN;
            }
            double mean = Mean(values, number);
            double sum = 0.0;
            for (int index = 0; index < number; index++)
            {
                sum += Math.Pow(values[values.Count - 1 - index] - mean, 2.0);
            }

            return sum / (number - 1);
        }

        public static double STD(List<double> values, int number)
        {
            return Math.Sqrt(Variance(values, number));
        }

        public static double Sharpe(List<double> values, int number)
        {
            throw new NotImplementedException();
        }

        public static double MDD(List<double> values, int number)
        {
            throw new NotImplementedException();
        }
    }
}
