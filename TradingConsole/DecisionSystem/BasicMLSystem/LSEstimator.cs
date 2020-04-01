namespace FinancialStructures.Mathematics
{
    /// <summary>
    /// Holds data on least squares estimator for a matrix of data inputs and corresponding y values.
    /// </summary>
    public class LSEstimator : IEstimator
    {
        private double[] Estimator;

        public double[] GetEstimator
        {
            get { return Estimator; }
        }

        public LSEstimator(double[,] data, double[] values)
        {
            GenerateEstimator(data, values);
        }

        public double Evaluate(double[] point)
        {
            if (Estimator.Length != point.Length)
            {
                return double.NaN;
            }
            double value = 0.0;
            for (int index = 0; index < Estimator.Length; index++)
            {
                value += Estimator[index] * point[index];
            }

            return value;
        }

        private void GenerateEstimator(double[,] data, double[] values)
        {
            var XTY = MatrixFunctions.VectorMultiply(MatrixFunctions.Transpose(data), values);
            Estimator = MatrixFunctions.VectorMultiply(MatrixFunctions.Inverse(MatrixFunctions.XTX(data)), XTY);
        }
    }
}
