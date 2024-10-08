using System;
using Xunit;

namespace Indexer.Tests
{
    public class CosineSimilarityTests
    {
        private readonly CosineSimilarity _cosineSimilarity;

        public CosineSimilarityTests()
        {
            _cosineSimilarity = new CosineSimilarity();
        }

        [Fact]
        public void ComputeSimilarity_SameLengthVectors_ReturnsCorrectValue()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0, 3.0 };
            double[] vectorB = { 4.0, 5.0, 6.0 };

            // Act
            double result = _cosineSimilarity.ComputeSimilarity(vectorA, vectorB);

            // Assert
            double expectedDotProduct = 1.0 * 4.0 + 2.0 * 5.0 + 3.0 * 6.0; // 32
            double expectedMagnitudeA = Math.Sqrt(1.0 * 1.0 + 2.0 * 2.0 + 3.0 * 3.0); // ~3.74
            double expectedMagnitudeB = Math.Sqrt(4.0 * 4.0 + 5.0 * 5.0 + 6.0 * 6.0); // ~8.77
            double expected = expectedDotProduct / (expectedMagnitudeA * expectedMagnitudeB); // ~0.97

            Assert.Equal(expected, result, 2); // Allowing 2 decimal places precision
        }

        [Fact]
        public void ComputeSimilarity_VectorsWithZeroMagnitude_ReturnsZero()
        {
            // Arrange
            double[] vectorA = { 0.0, 0.0, 0.0 };
            double[] vectorB = { 1.0, 2.0, 3.0 };

            // Act
            double result = _cosineSimilarity.ComputeSimilarity(vectorA, vectorB);  //Division by 0

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ComputeSimilarity_IdenticalVectors_ReturnsOne()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0, 3.0 };
            double[] vectorB = { 1.0, 2.0, 3.0 };

            // Act
            double result = _cosineSimilarity.ComputeSimilarity(vectorA, vectorB);

            // Assert
            Assert.Equal(1.0, result);
        }

        [Fact]
        public void ComputeSimilarity_VectorsOfDifferentLength_PrintsErrorAndReturnsZero()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0 };
            double[] vectorB = { 1.0, 2.0, 3.0 };

            SearchEngine cosineTest = new CosineSimilarity();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => cosineTest.ComputeSimilarity(vectorA, vectorB));

            // Optionally, you can check the exception message as well
            Assert.Equal("Vectors must have the same length for cosine similarity.", exception.Message);
        }

        [Fact]
        public void ComputeSimilarity_OrthogonalVectors_ReturnsZero()
        {
            // Arrange
            double[] vectorA = { 1.0, 0.0 };
            double[] vectorB = { 0.0, 1.0 };

            // Act
            double result = _cosineSimilarity.ComputeSimilarity(vectorA, vectorB);

            // Assert
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ComputeSimilarity_OneVectorIsZero_ReturnsZero()
        {
            // Arrange
            double[] vectorA = { 0.0, 0.0, 0.0 };
            double[] vectorB = { 0.0, 0.0, 0.0 };

            // Act
            double result = _cosineSimilarity.ComputeSimilarity(vectorA, vectorB);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
