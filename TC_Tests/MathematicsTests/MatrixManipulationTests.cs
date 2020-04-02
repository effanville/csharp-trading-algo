using NUnit.Framework;
using FinancialStructures.Mathematics;

namespace TC_Tests
{
    public class MatrixManipulationTests
    {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(7, 7)]
        public void IdentityCorrect(int n, int expectedMatrixIndex)
        {
            Assert.AreEqual(MatrixTestHelper.GetMatrix(expectedMatrixIndex).Matrix, MatrixFunctions.Identity(n));
        }

        [Test]
        public void TransposeCorrect([Values(1, 2, 3, 4, 5, 6, 7)] int expectedMatrixIndex)
        {
            var matrix = MatrixTestHelper.GetMatrix(expectedMatrixIndex);
            Assert.AreEqual(matrix.Transpose, MatrixFunctions.Transpose(matrix.Matrix));
        }

        [Test]
        public void LUDecompCorrect([Values(1, 2, 3, 4, 5, 6, 7, 8)] int expectedMatrixIndex)
        {
            var matrix = MatrixTestHelper.GetMatrix(expectedMatrixIndex);
            var lUDecomposition = new LUDecomposition(matrix.Matrix);
            var product = MatrixFunctions.Multiply(lUDecomposition.LowerDecomp, lUDecomposition.UpperDecomp);
            Assertions.AreEqual(matrix.Matrix, product, 1e-3, "products wrong");
            Assert.AreEqual(lUDecomposition.Invertible, true);
            Assertions.AreEqual(matrix.Upper, lUDecomposition.UpperDecomp, 1e-3, "Upper wrong");
            Assertions.AreEqual(matrix.Lower, lUDecomposition.LowerDecomp, 1e-3, "Lower wrong");
        }

        [Test]
        public void InverseCorrect([Values(1, 2, 3, 4, 5, 6, 7, 8)] int expectedMatrixIndex)
        {
            var matrix = MatrixTestHelper.GetMatrix(expectedMatrixIndex);
            Assertions.AreEqual(matrix.Inverse, MatrixFunctions.Inverse(matrix.Matrix), 1e-6);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(3, 5)]
        [TestCase(3, 6)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(4, 5)]
        [TestCase(4, 6)]
        [TestCase(5, 3)]
        [TestCase(5, 4)]
        [TestCase(5, 5)]
        [TestCase(5, 6)]
        [TestCase(6, 3)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 7)]
        public void MultiplyOK(int matrixIndex, int matrix2Index)
        {
            var matrix = MatrixTestHelper.GetMatrix(matrixIndex);
            var vector = MatrixTestHelper.GetMatrix(matrix2Index);
            var expected = MatrixTestHelper.GetExpectedMatrixProduct(matrixIndex, matrix2Index);
            Assert.AreEqual(expected, MatrixFunctions.Multiply(matrix.Matrix, vector.Matrix));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(3, 5)]
        [TestCase(3, 6)]
        [TestCase(3, 7)]
        [TestCase(4, 4)]
        [TestCase(4, 5)]
        [TestCase(4, 6)]
        [TestCase(4, 7)]
        [TestCase(5, 4)]
        [TestCase(5, 5)]
        [TestCase(5, 6)]
        [TestCase(5, 7)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(6, 7)]
        public void VectorMatrixMultiplyOK(int matrixIndex, int vectorIndex)
        {
            var matrix = MatrixTestHelper.GetMatrix(matrixIndex);
            var vector = MatrixTestHelper.GetVector(vectorIndex);
            var expected = MatrixTestHelper.GetExpectedProduct(matrixIndex, vectorIndex);
            Assert.AreEqual(expected, MatrixFunctions.VectorMultiply(matrix.Matrix, vector));
        }

        [Test]
        public void ComputeXTXOK([Values(1, 2, 3, 4, 5, 6, 7)] int expectedMatrixIndex)
        {
            var matrix = MatrixTestHelper.GetMatrix(expectedMatrixIndex);
            Assert.AreEqual(matrix.XTX, MatrixFunctions.XTX(matrix.Matrix));
        }
    }
}
