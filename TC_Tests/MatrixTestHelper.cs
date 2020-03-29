namespace TC_Tests
{
    public class TestMatrixValues
    {
        public double[,] Matrix;
        public double[,] Transpose;
        public double[,] XTX;
        public double[,] Lower;
        public double[,] Upper;
        public double[] Pivot;
        public double[,] Inverse;
    }
    public static class MatrixTestHelper
    {
        public static double[] GetVector(int vectorIndex)
        {
            switch (vectorIndex)
            {
                case (1):
                    return new double[] { 1 };
                case (2):
                    return new double[] { 1, 0 };
                case (3):
                    return new double[] { 0, 1 };
                case (4):
                    return new double[] { 1, 0, 0 };
                case (5):
                    return new double[] { 0, 0, 1 };
                case (6):
                    return new double[] { 0, 1, 0 };
                case (7):
                    {
                        return new double[] { 1, 2, 3 };
                    }
                default:
                    return new double[] { 1, 2, 3 };
            }
        }

        public static double[] GetExpectedProduct(int matrixIndex, int vectorIndex)
        {
            switch (matrixIndex)
            {
                case 1:
                    switch (vectorIndex)
                    {
                        case 1:
                            return new double[] { 1 };
                        default:
                            return new double[] { 0 };
                    }
                case 2:
                    switch (vectorIndex)
                    {
                        case 2:
                            return new double[] { 1, 0 };
                        case 3:
                            return new double[] { 0, 1 };
                        default:
                            return new double[] { 0 };
                    }
                case 3:
                    switch (vectorIndex)
                    {
                        case 4:
                            return new double[] { 1, 0, 0 };
                        case 5:
                            return new double[] { 0, 0, 1 };
                        case 6:
                            return new double[] { 0, 1, 0 };
                        case 7:
                            return new double[] { 1, 2, 3 };
                        default:
                            return new double[] { 0 };
                    }
                case 4:
                    switch (vectorIndex)
                    {
                        case 4:
                            return new double[] { 1, 0, 0 };
                        case 5:
                            return new double[] { 1, 0, 1 };
                        case 6:
                            return new double[] { 0, 1, 0 };
                        case 7:
                            return new double[] { 4, 2, 3 };
                        default:
                            return new double[] { 0 };
                    }
                case 5:
                    switch (vectorIndex)
                    {
                        case 4:
                            return new double[] { 1, 4, 7 };
                        case 5:
                            return new double[] { 3, 6, 9 };
                        case 6:
                            return new double[] { 2, 5, 7 };
                        case 7:
                            return new double[] { 14, 32, 48 };
                        default:
                            return new double[] { 0 };
                    }
                case 6:
                    switch (vectorIndex)
                    {
                        case 4:
                            return new double[] { 7, 9, 4 };
                        case 5:
                            return new double[] { 3, -7.2, -2.3 };
                        case 6:
                            return new double[] { 4.3, 2.2, 88 };
                        case 7:
                            return new double[] { 24.6, -8.2000000000000011, 173.1 };
                        default:
                            return new double[] { 0 };
                    }
                case 7:
                    switch (vectorIndex)
                    {
                        default:
                            return new double[] { 0 };
                    }
                default:
                    return new double[] { 0 };
            }
        }

        public static double[,] GetExpectedMatrixProduct(int matrix1Index, int matrix2Index)
        {
            switch (matrix1Index)
            {
                case 1:
                    switch (matrix2Index)
                    {
                        case 1:
                            return new double[,] { { 1 } };
                        default:
                            return new double[,] { { 0 } };
                    }
                case 2:
                    switch (matrix2Index)
                    {
                        case 2:
                            return new double[,] { { 1, 0 }, { 0, 1 } };
                        default:
                            return new double[,] { { 0 } };
                    }
                case 3:
                    switch (matrix2Index)
                    {
                        case 3:
                            return new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                        case 4:
                            return new double[,] { { 1, 0, 1 }, { 0, 1, 0 }, { 0, 0, 1 } };
                        case 5:
                            return new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 7, 9 } };
                        case 6:
                            return new double[,] { { 7, 4.3, 3 }, { 9, 2.2, -7.2 }, { 4, 88, -2.3 } };
                        default:
                            return new double[,] { { 0 } };
                    }
                case 4:
                    switch (matrix2Index)
                    {
                        case 3:
                            return new double[,] { { 1, 0, 1 }, { 0, 1, 0 }, { 0, 0, 1 } };
                        case 4:
                            return new double[,] { { 1, 0, 2 }, { 0, 1, 0 }, { 0, 0, 1 } };
                        case 5:
                            return new double[,] { { 8, 9, 12 }, { 4, 5, 6 }, { 7, 7, 9 } };
                        case 6:
                            return new double[,] { { 11, 92.3, 0.70000000000000018 }, { 9, 2.2, -7.2 }, { 4, 88, -2.3 } };
                        default:
                            return new double[,] { { 0 } };
                    }
                case 5:
                    switch (matrix2Index)
                    {
                        case 3:
                            return new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 7, 9 } };
                        case 4:
                            return new double[,] { { 1, 2, 4 }, { 4, 5, 10 }, { 7, 7, 16 } };
                        case 5:
                            return new double[,] { { 30, 33, 42 }, { 66, 75, 96 }, { 98, 112, 144 } };
                        case 6:
                            return new double[,] { { 37, 272.7, -18.3 }, { 97, 556.2, -37.8 }, { 148, 837.5, -50.099999999999994 } };
                        default:
                            return new double[,] { { 0 } };
                    }
                case 6:
                    switch (matrix2Index)
                    {
                        case 3:
                            return new double[,] { { 7, 4.3, 3 }, { 9, 2.2, -7.2 }, { 4, 88, -2.3 } };
                        case 4:
                            return new double[,] { { 7, 4.3, 10 }, { 9, 2.2, 1.7999999999999998 }, { 4, 88, 1.7000000000000002 } };
                        case 5:
                            return new double[,] { { 45.2, 56.5, 73.8 }, { -32.599999999999994, -21.4, -24.599999999999994 }, { 339.9, 431.9, 519.3 } };
                        case 6:
                            return new double[,] { { 99.699999999999989, 303.56, -16.86 }, { 54, -590.06000000000006, 27.72 }, { 810.8, 8.4000000000000341, -616.31000000000006 } };
                        default:
                            return new double[,] { { 0 } };
                    }
                case 7:
                    switch (matrix2Index)
                    {
                        case 7:
                            return new double[,] { { 1, 0, 0, 0, 0, 0, 0 },
                                                     { 0, 1, 0, 0, 0, 0, 0 },
                                                     { 0, 0, 1, 0, 0, 0, 0 },
                                                     { 0, 0, 0, 1, 0, 0, 0 },
                                                     { 0, 0, 0, 0, 1, 0, 0 },
                                                     { 0, 0, 0, 0, 0, 1, 0 },
                                                     { 0, 0, 0, 0, 0, 0, 1 } };
                        default:
                            return new double[,] { { 0 } };
                    }
                default:
                    return new double[,] { { 0 } };
            }
        }

        public static TestMatrixValues GetMatrix(int matrixIndex)
        {
            switch (matrixIndex)
            {
                case (1):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1 } },
                            Transpose = new double[,] { { 1 } },
                            XTX = new double[,] { { 1 } },
                            Lower = new double[,] { { 1 } },
                            Upper = new double[,] { { 1 } },
                            Inverse = new double[,] { { 1 } }
                        };
                    }
                case (2):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1, 0 }, { 0, 1 } },
                            Transpose = new double[,] { { 1, 0 }, { 0, 1 } },
                            XTX = new double[,] { { 1, 0 }, { 0, 1 } },
                            Lower = new double[,] { { 1, 0 }, { 0, 1 } },
                            Upper = new double[,] { { 1, 0 }, { 0, 1 } },
                            Inverse = new double[,] { { 1, 0 }, { 0, 1 } }
                        };
                    }
                case (3):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Transpose = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            XTX = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Lower = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Upper = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Inverse = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }
                        };
                    }
                case (4):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1, 0, 1 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Transpose = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 1, 0, 1 } },
                            XTX = new double[,] { { 1, 0, 1 }, { 0, 1, 0 }, { 1, 0, 2 } },
                            Lower = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Upper = new double[,] { { 1, 0, 1 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Inverse = new double[,] { { 1, 0, -1 }, { 0, 1, 0 }, { 0, 0, 1 } }
                        };
                    }
                case (5):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 7, 9 } },
                            Transpose = new double[,] { { 1, 4, 7 }, { 2, 5, 7 }, { 3, 6, 9 } },
                            XTX = new double[,] { { 66, 71, 90 }, { 71, 78, 99 }, { 90, 99, 126 } },
                            Lower = new double[,] { { 1, 0, 0 }, { 4, 1, 0 }, { 7, 2.333333333333333333333333333, 1 } },
                            Upper = new double[,] { { 1, 2, 3 }, { 0, -3, -6 }, { 0, 0, 2 } },
                            Inverse = new double[,] { { -0.49999999999999956, -0.5, 0.5 }, { -1.0000000000000007, 2, -1 }, { 1.166666666666667, -1.1666666666666666666666, 0.5 } }
                        };
                    }
                case (6):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 7, 4.3, 3 }, { 9, 2.2, -7.2 }, { 4, 88, -2.3 } },
                            Transpose = new double[,] { { 7, 9, 4 }, { 4.3, 2.2, 88 }, { 3, -7.2, -2.3 } },
                            XTX = new double[,] { { 146, 401.9, -53 }, { 401.9, 7767.33, -205.33999999999997 }, { -53, -205.33999999999997, 66.13 } },
                            Lower = new double[,] { { 1, 0, 0 }, { 1.2857142857142858, 1, 0 }, { 0.5714285714285714, -25.699570815450645, 1 } },
                            Upper = new double[,] { { 7, 4.3, 3 }, { 0, -3.3285714285714283, -11.057142857142857 }, { 0, 0, -288.17811158798287 } },
                            Pivot = new double[] { 2, 3, 1 },
                            Inverse = new double[,] { { 0.0936087, 0.0407905, -0.00559382 }, { -0.00120634, -0.00418494, 0.0115272 }, { 0.116642, -0.0891795, -0.00347008 } }
                        };
                    }
                case (7):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1, 0, 0, 0, 0, 0, 0 },
                                                     { 0, 1, 0, 0, 0, 0, 0 },
                                                     { 0, 0, 1, 0, 0, 0, 0 },
                                                     { 0, 0, 0, 1, 0, 0, 0 },
                                                     { 0, 0, 0, 0, 1, 0, 0 },
                                                     { 0, 0, 0, 0, 0, 1, 0 },
                                                     { 0, 0, 0, 0, 0, 0, 1 } },
                            Transpose = new double[,] { { 1, 0, 0, 0, 0, 0, 0 },
                                                        { 0, 1, 0, 0, 0, 0, 0 },
                                                        { 0, 0, 1, 0, 0, 0, 0 },
                                                        { 0, 0, 0, 1, 0, 0, 0 },
                                                        { 0, 0, 0, 0, 1, 0, 0 },
                                                     { 0, 0, 0, 0, 0, 1, 0 },
                                                     { 0, 0, 0, 0, 0, 0, 1 } },
                            XTX = new double[,] { { 1, 0, 0, 0, 0, 0, 0 },
                                                     { 0, 1, 0, 0, 0, 0, 0 },
                                                     { 0, 0, 1, 0, 0, 0, 0 },
                                                     { 0, 0, 0, 1, 0, 0, 0 },
                                                     { 0, 0, 0, 0, 1, 0, 0 },
                                                     { 0, 0, 0, 0, 0, 1, 0 },
                                                     { 0, 0, 0, 0, 0, 0, 1 } },
                            Lower = new double[,] { { 1, 0, 0, 0, 0, 0, 0 },
                                                     { 0, 1, 0, 0, 0, 0, 0 },
                                                     { 0, 0, 1, 0, 0, 0, 0 },
                                                     { 0, 0, 0, 1, 0, 0, 0 },
                                                     { 0, 0, 0, 0, 1, 0, 0 },
                                                     { 0, 0, 0, 0, 0, 1, 0 },
                                                     { 0, 0, 0, 0, 0, 0, 1 } },
                            Upper = new double[,] { { 1, 0, 0, 0, 0, 0, 0 },
                                                     { 0, 1, 0, 0, 0, 0, 0 },
                                                     { 0, 0, 1, 0, 0, 0, 0 },
                                                     { 0, 0, 0, 1, 0, 0, 0 },
                                                     { 0, 0, 0, 0, 1, 0, 0 },
                                                     { 0, 0, 0, 0, 0, 1, 0 },
                                                     { 0, 0, 0, 0, 0, 0, 1 } },
                            Inverse = new double[,] { { 1, 0, 0, 0, 0, 0, 0 },
                                                     { 0, 1, 0, 0, 0, 0, 0 },
                                                     { 0, 0, 1, 0, 0, 0, 0 },
                                                     { 0, 0, 0, 1, 0, 0, 0 },
                                                     { 0, 0, 0, 0, 1, 0, 0 },
                                                     { 0, 0, 0, 0, 0, 1, 0 },
                                                     { 0, 0, 0, 0, 0, 0, 1 } }
                        };
                    }
                case (8):
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1, 2.2, 3, -4.1 }, { 4, 3, 6, 8 }, { 6.7, 5, -2.2, 1 }, { 1, -1, 0.2, 45 } },
                            Lower = new double[,] { { 1, 0, 0, 0 }, { 4, 1, 0, 0 }, { 6.7, 1.6793103448275863, 1, 0 }, { 1, 0.55172413793103448, -0.041748942172073353, 1 } },
                            Upper = new double[,] { { 1, 2.2, 3, -4.1 }, { 0, -5.8000000000000007, -6, 24.4 }, { 0, 0, -12.224137931034482, -12.505172413793105 }, { 0, 0, 0, 35.115853314527506 } },
                            Pivot = new double[] { 3, 4, 2, 1 },
                            Inverse = new double[,] { { -0.668029, 0.358001, 0.0539901, -0.125709 }, { 0.87218, -0.407763, 0.0908859, 0.149937 }, { -0.0365905, 0.155492, -0.0830216, -0.0291319 }, { 0.0343895, -0.0177081, 0.00118889, 0.0284772 } }
                        };
                    }
                default:
                    {
                        return new TestMatrixValues()
                        {
                            Matrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Transpose = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                            Inverse = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }
                        };
                    }
            }
        }
    }
}
