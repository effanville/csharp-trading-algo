using NUnit.Framework;
using FinancialStructures.Mathematics;
using System;

namespace TC_Tests
{
    internal class EstimatorValues
    {
        public double[,] data;
        public double[] rhs;
        public double[] expectedEstimator;

        public static EstimatorValues GetValues(int index)
        {
            switch (index)
            {
                case (1):
                    return new EstimatorValues()
                    {
                        data = new double[,] { { 1, 0 }, { 0, 1 } },
                        rhs = new double[] { 1, 1 },
                        expectedEstimator = new double[] { 1, 1 }
                    };
                case (2):
                    return new EstimatorValues()
                    {
                        data = new double[,] { { 2, 1 }, { -1, 1 } },
                        rhs = new double[] { 5, 2 },
                        expectedEstimator = new double[] { 1, 3 }
                    };
                case (3):
                    return new EstimatorValues()
                    {
                        data = new double[,] { { 2, 1, -2 }, { 1, -1, -1 }, { 1, 1, 3 } },
                        rhs = new double[] { 3, 0, 12 },
                        expectedEstimator = new double[] { 3.5, 1, 2.5 }
                    };
                case (4):
                    var dataValues = new double[50, 3];
                    var yValues = new double[50];
                    for (int i = 0; i < 50; i++)
                    {
                        dataValues[i, 0] = 1;
                        dataValues[i, 1] = i * 0.1;
                        dataValues[i, 2] = i * i * 0.01;
                        yValues[i] = dataValues[i, 0] + dataValues[i, 1] * 2.2 + dataValues[i, 2] * 4;
                    }
                    return new EstimatorValues()
                    {
                        data = dataValues,
                        rhs = yValues,
                        expectedEstimator = new double[] { 1, 2.2, 4 }
                    };
                case (5):
                    Random rnd = new Random();
                    var dataValuesRnd = new double[50, 3];
                    var yValuesRnd = new double[50];
                    for (int i = 0; i < 50; i++)
                    {
                        dataValuesRnd[i, 0] = 1 + rnd.NextDouble();
                        dataValuesRnd[i, 1] = i * 0.1 + rnd.NextDouble();
                        dataValuesRnd[i, 2] = i * i * 0.01 + rnd.NextDouble();
                        yValuesRnd[i] = dataValuesRnd[i, 0] + dataValuesRnd[i, 1] * 2.2 + dataValuesRnd[i, 2] * 4;
                    }
                    return new EstimatorValues()
                    {
                        data = dataValuesRnd,
                        rhs = yValuesRnd,
                        expectedEstimator = new double[] { 1, 2.2, 4 }
                    };
                case (6):
                    Random rnd2 = new Random();
                    var dataValuesRnd2 = new double[400, 10];
                    var yValuesRnd2 = new double[400];
                    for (int i = 0; i < 20; i++)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            dataValuesRnd2[20 * i + j, 0] = 1 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 1] = i * 0.1 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 2] = j * 0.1 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 3] = i * j * 0.01 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 4] = i * i * 0.01 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 5] = j * j * 0.01 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 6] = i * j * j * 0.001 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 7] = i * i * j * 0.001 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 8] = i * i * i * 0.001 + rnd2.NextDouble();
                            dataValuesRnd2[20 * i + j, 9] = j * j * j * 0.001 + rnd2.NextDouble();
                            yValuesRnd2[20 * i + j] = dataValuesRnd2[20 * i + j, 0] + dataValuesRnd2[20 * i + j, 1] * 2.2 + dataValuesRnd2[20 * i + j, 2] * -4 + dataValuesRnd2[20 * i + j, 3] * 3 + dataValuesRnd2[20 * i + j, 4] * 0.01 + dataValuesRnd2[20 * i + j, 5] * -0.2 + dataValuesRnd2[20 * i + j, 6] * 7 + dataValuesRnd2[20 * i + j, 7] * 2.3 + dataValuesRnd2[20 * i + j, 8] + dataValuesRnd2[20 * i + j, 9];
                        }
                    }
                    return new EstimatorValues()
                    {
                        data = dataValuesRnd2,
                        rhs = yValuesRnd2,
                        expectedEstimator = new double[] { 1, 2.2, -4, 3, 0.01, -0.2, 7, 2.3, 1, 1 }
                    };
                default:
                    return new EstimatorValues()
                    {
                        data = new double[,] { { 1, 0 }, { 0, 1 } },
                        rhs = new double[] { 1, 1 },
                        expectedEstimator = new double[] { 1, 1 }
                    };
            }
        }
    }

    public class LSEstimatorTests
    {
        [Test]
        public void LSECorrect([Values(1, 2, 3, 4, 5, 6)] int valuesIndex)
        {
            var estimatorValues = EstimatorValues.GetValues(valuesIndex);
            var estimator = new LSEstimator(estimatorValues.data, estimatorValues.rhs);
            Assertions.AreEqual(estimatorValues.expectedEstimator, estimator.Estimator, 1e-6, "Expected Estimator not correct");
        }
    }
}
