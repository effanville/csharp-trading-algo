namespace TradingConsole.DecisionSystem.BasicMLSystem
{
    public class LUDecomposition
    {
        double[,] UpperDecomp;

        double[,] LowerDecomp;

        double[] PivotValues;

        public bool Invertible = true;

        public LUDecomposition(double[,] matrix)
        {
            GenerateLUDecomp(matrix);
        }
        public double[,] Inverse()
        {
            int size = UpperDecomp.GetLength(0);
            var inverse = new double[size, size];
            var X = new double[size];
            var Y = new double[size];
            var identity = MatrixFunctions.Identity(size);
            double t = 0.0;

            //Solving Ly = Pb.
            for (int i = 0; i < size; i++)
            {
                for (int n = 0; n < size; n++)
                {
                    t = 0;
                    for (int m = 0; m < n; m++)
                    {
                        t += LowerDecomp[n, m] * Y[m];
                    }

                    Y[n] = identity[i, i] - t;
                }
                //Solving Ux = y.
                for (int n = size - 1; n >= 0; n--)
                {
                    t = 0;
                    for (int m = n + 1; m < size; m++)
                    {
                        t += UpperDecomp[n, m] * X[m];
                    }

                    X[n] = (Y[n] - t) / UpperDecomp[n, n];

                }//Now, X contains the solution.

                for (int j = 0; j < size; j++)
                {
                    identity[j, i] = X[j]; //Copying 'X' into the same row of 'B'.
                } //Now, 'B' the transpose of the inverse of 'A'.
            }
            /* Copying transpose of 'B' into 'LU', which would the inverse of 'A'. */
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    inverse[i, j] = identity[i, j];
                }
            }

            return inverse;
        }

        private void GenerateLUDecomp(double[,] matrix)
        {
            if (!matrix.GetLength(0).Equals(matrix.GetLength(1)))
            {
                Invertible = false;
                return;
            }

            int size = matrix.GetLength(0);
            PivotValues = new double[size];
            UpperDecomp = new double[size, size];
            LowerDecomp = new double[size, size];
            double p, t;

            /* Finding the pivot of the LUP decomposition. */
            for (int i = 0; i < size; i++)
            {
                PivotValues[i] = i;
            } //Initializing.

            for (int i = 0; i < size; i++)
            {
                p = 0;
                t = matrix[i, i];

                if (t != 0)
                {
                    p = t;
                }

                if (p == 0)
                {
                    Invertible = false;
                    return;
                }

                for (int k = i; k < size; k++) //Performing subtraction to decompose A as LU.
                {
                    double sum = 0.0;
                    for (int j = 0; j < i; j++)
                    {
                        sum += LowerDecomp[i, j] * UpperDecomp[j, k];
                    }

                    UpperDecomp[i, k] = matrix[i, k] - sum;
                }

                for (int k = i; k < size; k++) //Performing subtraction to decompose A as LU.
                {
                    double sum = 0.0;
                    for (int j = 0; j < i; j++)
                    {
                        sum += LowerDecomp[k, j] * UpperDecomp[j, i];
                    }

                    LowerDecomp[k, i] = (matrix[k, i] - sum) / UpperDecomp[i, i];
                }
            }
        }
    }
}
