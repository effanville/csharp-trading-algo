namespace TradingConsole.DecisionSystem.BasicMLSystem
{
    public interface IEstimator
    {
        double[] GetEstimator { get; }
        double Evaluate(double[] point);
    }
}
