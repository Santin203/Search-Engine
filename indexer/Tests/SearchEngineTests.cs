using System;
using System.IO;
using Xunit;

namespace Indexer.Tests
{
    public class SearchEngineTests
    {
        private readonly StringWriter _output; // To capture Console output
        private readonly SimpleSearchEngine _searchEngine;

        public SearchEngineTests()
        {
            _output = new StringWriter();
            Console.SetOut(_output); // Redirect Console output to capture it
            _searchEngine = new SimpleSearchEngine();
        }

        [Fact]
        public void CheckVectorLength_VectorsOfDifferentLength_PrintsErrorMessage()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0, 3.0 };
            double[] vectorB = { 1.0, 2.0 };

            SearchEngine cosineTest = new CosineSimilarity();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => cosineTest.ComputeSimilarity(vectorA, vectorB));

            // Optionally, you can check the exception message as well
            Assert.Equal("Vectors must have the same length for cosine similarity.", exception.Message);
        }

        [Fact]
        public void CheckVectorLength_VectorsOfSameLength_NoErrorMessage()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0, 3.0 };
            double[] vectorB = { 4.0, 5.0, 6.0 };

            // Act
            double result = _searchEngine.ComputeSimilarity(vectorA, vectorB);

            // Assert
            Assert.Equal(1,result);
        }

        [Fact]
        public void ComputeSimilarity_VectorsOfSameLength_ReturnsNonZeroSimilarity()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0, 3.0 };
            double[] vectorB = { 4.0, 5.0, 6.0 };

            // Act
            var similarity = _searchEngine.ComputeSimilarity(vectorA, vectorB);

            // Assert
            Assert.Equal(1.0, similarity);
        }

        [Fact]
        public void ComputeSimilarity_VectorsOfDifferentLength_ReturnsZeroSimilarity()
        {
            // Arrange
            double[] vectorA = { 1.0, 2.0, 3.0 };
            double[] vectorB = { 1.0, 2.0 };

            // Act
            var similarity = _searchEngine.ComputeSimilarity(vectorA, vectorB);

            // Assert
            Assert.Equal(0.0, similarity);
        }

        // Cleanup to restore original Console output
        public void Dispose()
        {
            Console.SetOut(Console.Out); // Restore original Console output
            _output.Dispose();
        }
    }

    //Dummy class to always know the results of ComputeSimilarity
    public class SimpleSearchEngine : SearchEngine
    {
        public override double ComputeSimilarity(double[] vectorA, double[] vectorB)
        {
            checkVectorLength(vectorA, vectorB);
            // For simplicity, we'll just return a dummy similarity value.
            // You can implement a real similarity measure as needed.
            return vectorA.Length == vectorB.Length ? 1.0 : 0.0;
        }
    }
}
