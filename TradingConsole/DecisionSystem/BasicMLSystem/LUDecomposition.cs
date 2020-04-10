namespace FinancialStructures.Mathematics
{
    public class LUDecomposition
    {
        private int Size;
        internal double[,] UpperDecomp;

        internal double[,] LowerDecomp;

        int[] PivotValues;

        public bool Invertible = true;
        public bool Square = true;

        public LUDecomposition(double[,] matrix)
        {
            GenerateLUDecomp(matrix);
        }

        // here solving the equation Ax = b.
        // solve first Ly = b,
        // then solve Ux = y
        // x is then the output.
        public double[] LinearSolve(double[] b)
        {
            double[] y = new double[Size];
            double[] x = new double[Size];
            int vectorRowIndex, firstNonZeroIndex = 0, pivotIndex, matrixColumnIndex;
            double sum;
            for (vectorRowIndex = 0; vectorRowIndex < Size; vectorRowIndex++)
            {
                pivotIndex = PivotValues[vectorRowIndex];
                sum = b[pivotIndex];
                if (firstNonZeroIndex > -1)
                {
                    for (matrixColumnIndex = firstNonZeroIndex; matrixColumnIndex < vectorRowIndex; matrixColumnIndex++)
                    {
                        sum -= LowerDecomp[vectorRowIndex, matrixColumnIndex] * y[matrixColumnIndex];
                    }
                }
                else if (sum > 0)
                {
                    firstNonZeroIndex = vectorRowIndex;
                }

                y[vectorRowIndex] = sum;
            }

            for (vectorRowIndex = Size - 1; vectorRowIndex > -1; vectorRowIndex--)
            {
                sum = y[vectorRowIndex];
                for (matrixColumnIndex = vectorRowIndex + 1; matrixColumnIndex < Size; matrixColumnIndex++)
                {
                    sum -= UpperDecomp[vectorRowIndex, matrixColumnIndex] * x[matrixColumnIndex];
                }

                x[vectorRowIndex] = sum / UpperDecomp[vectorRowIndex, vectorRowIndex];
            }

            return x;
        }

        public double[,] Inverse()
        {
            var inverse = new double[Size, Size];
            var col = new double[Size];
            for (int j = 0; j < Size; j++)
            {
                for (int i = 0; i < Size; i++)
                {
                    col[i] = 0;
                }

                col[j] = 1;
                col = LinearSolve(col);
                for (int i = 0; i < Size; i++)
                {
                    inverse[i, j] = col[i];
                }
            }

            return inverse;
        }

        private void GenerateLUDecomp(double[,] matrix)
        {
            if (!matrix.GetLength(0).Equals(matrix.GetLength(1)))
            {
                Invertible = false;
                Square = false;
                return;
            }

            Size = matrix.GetLength(0);
            PivotValues = new int[Size];
            UpperDecomp = new double[Size, Size];
            LowerDecomp = new double[Size, Size];

            // Initialising the pivot values to the identity permutation
            for (int n = 0; n < Size; n++)
            {
                LowerDecomp[n, n] = 1;
                PivotValues[n] = n;
            }

            double sum = 0.0;

            for (int columnIndex = 0; columnIndex < Size; columnIndex++)
            {
                for (int upperRowIndex = 0; upperRowIndex < columnIndex + 1; upperRowIndex++)
                {
                    sum = matrix[upperRowIndex, columnIndex];
                    for (int k = 0; k < upperRowIndex; k++)
                    {
                        sum -= LowerDecomp[upperRowIndex, k] * UpperDecomp[k, columnIndex];
                    }

                    UpperDecomp[upperRowIndex, columnIndex] = sum;
                }

                if (UpperDecomp[columnIndex, columnIndex].Equals(0.0))
                {
                    Invertible = false;
                    return;
                }

                for (int lowerRowIndex = columnIndex + 1; lowerRowIndex < Size; lowerRowIndex++)
                {
                    sum = matrix[lowerRowIndex, columnIndex];
                    for (int k = 0; k < columnIndex; k++)
                    {
                        sum -= LowerDecomp[lowerRowIndex, k] * UpperDecomp[k, columnIndex];
                    }

                    LowerDecomp[lowerRowIndex, columnIndex] = sum / UpperDecomp[columnIndex, columnIndex];
                }
            }
        }
    }
}
