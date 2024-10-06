using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Tests
{
    public class TFIDFTests : TFIDF
    {
        public TFIDFTests() : base() {}

        [Fact]
        public void Fit_BuildsVocabularyAndComputesIdf()
        {
            // Arrange
            string[] documents = {
                "apple banana apple",
                "banana orange"
            };

            // Act
            Fit(documents);

            // Assert
            Assert.Equal(3, Vocabulary.Count); // There should be 3 unique words
            Assert.Contains("apple", Vocabulary.Keys);
            Assert.Contains("banana", Vocabulary.Keys);
            Assert.Contains("orange", Vocabulary.Keys);
            Assert.Equal(3, Idf.Count); 
        }

        [Fact]
        public void ComputeIdf_CorrectlyComputesIdfValues()
        {
            // Arrange
            string[] documents = {
                "apple banana apple",
                "banana orange"
            };
            Fit(documents);

            // Act
            ComputeIdf();

            // Assert
            Assert.True(Idf.ContainsKey("apple"));
            Assert.True(Idf.ContainsKey("banana"));
            Assert.True(Idf.ContainsKey("orange"));
            Assert.True(Idf["apple"] > 0); // IDF value for apple should be greater than 0
            Assert.True(Idf["banana"] > 0); // IDF value for banana should be greater than 0
            Assert.True(Idf["orange"] > 0); // IDF value for orange should be greater than 0
        }

        [Fact]
        public void ComputeVector_ReturnsCorrectTfidfValues()
        {
            // Arrange
            string[] documents = {
                "apple banana apple",
                "banana orange"
            };
            Fit(documents);
            ComputeIdf(); // Ensure IDF values are computed

            var words = new List<string> { "apple", "banana", "banana" };

            // Act
            var result = ComputeVector(words);

            // Assert
            Assert.Equal(2, result.Count); // Two unique words in the list
            Assert.True(result.ContainsKey("apple")); // TF-IDF should include "apple"
            Assert.True(result.ContainsKey("banana")); // TF-IDF should include "banana"
            Assert.True(result["apple"] > 0); // TF-IDF for "apple" should be greater than 0
            Assert.True(result["banana"] > 0); // TF-IDF for "banana" should be greater than 0
        }
    }
}
