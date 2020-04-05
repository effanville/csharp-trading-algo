namespace FinancialStructures.Mathematics
{
    /// <summary>
    /// Container for any estimator of values.
    /// </summary>
    public interface IEstimator
    {
        /// <summary>
        /// Returns the values one has estimated.
        /// </summary>
        double[] Estimator { get; }

        /// <summary>
        /// Evaluates the point using the estimator calculated.
        /// </summary>
        /// <param name="point">The value at which to evaluate at.</param>
        double Evaluate(double[] point);

        /// <summary>
        /// Calculates the values of the estimator from the data and expected values.
        /// </summary>
        /// <param name="data">The data values to use.</param>
        /// <param name="values">The expected outcomes for each row of the data matrix.</param>
        void GenerateEstimator(double[,] data, double[] values);
    }
}
