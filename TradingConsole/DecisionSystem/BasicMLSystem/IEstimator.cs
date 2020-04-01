namespace FinancialStructures.Mathematics
{
    public interface IEstimator
    {
        double[] GetEstimator { get; }
        double Evaluate(double[] point);
    }
}
