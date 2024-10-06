using System;
using Xunit;

namespace Indexer.Tests
{
    public class SearchitoTests : Searchito
    {
        private readonly Searchito _searchito;

        public SearchitoTests()
        {
            _searchito = new Searchito();
        }
        [Fact]
        public void ComputeSimilarity_CallsCalculatePearsonCorrelation()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0, 3.0 };
            double[] vectorB = { 1.0, 2.0, 3.0 };

            // Act
            var similarity = ComputeSimilarity(vectorA, vectorB);

            // Assert
            Assert.NotNull(similarity); // Ensure the similarity value is calculated
            Assert.Equal(1.0, _similarityMeasure, 5); // Expect perfect correlation for identical vectors
        }

        [Fact]
        public void GetFunction_ReturnsCorrectFunctionPoints()
        {
            // Arrange
            double[] vector = { 1.0, 2.0, 3.0 };

            // Act
            var result = GetFunction(vector, vector.Length);

            // Assert
            Assert.Equal(30, result[2]); // 10 * 3
            Assert.Equal(20, result[1]); // 10 * 2
            Assert.Equal(10, result[0]); // 10 * 1
        }

        [Fact]
        public void CalculatePearsonCorrelation_CalculatesCorrectCorrelation()
        {
            // Arrange
            double[] functionA = { 10, 20, 30 };
            double[] functionB = { 10, 20, 30 };

            // Act
            CalculatePearsonCorrelation(functionA, functionB);

            // Assert
            Assert.Equal(1.0, _similarityMeasure, 5); // Expect perfect correlation
        }

        [Fact]
        public void CalculatePearsonCorrelation_HandlesDivisionByZero()
        {
            // Arrange
            double[] functionA = { 0, 0, 0 }; // All values the same
            double[] functionB = { 0, 0, 0 }; // All values the same

            // Act
            CalculatePearsonCorrelation(functionA, functionB);

            // Assert
            Assert.Equal(0, _similarityMeasure); // Expect correlation to be zero due to division by zero
        }

        [Fact]
        public void ComputeSimilarity_VectorsOfDifferentLength_PrintsErrorAndReturnsZero()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0 };
            double[] vectorB = { 1.0, 2.0, 3.0 };

            // Act 
            double result = _searchito.ComputeSimilarity(vectorA, vectorB);
            // Assert
            Assert.True(result == 0.0);
        }
    }
}
