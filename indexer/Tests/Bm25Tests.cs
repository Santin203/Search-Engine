using System;
using System.Collections.Generic;
using Xunit;

namespace Indexer.Tests
{
    public class Bm25Tests : Bm25
    {
        [Fact]
        public void Fit_ShouldBuildVocabularyAndComputeIdf()
        {
            // Arrange
            string[] documents = { "word1 word2", "word2 word3" };

            // Act
            Fit(documents);

            // Assert
            Assert.NotEmpty(Vocabulary); // Vocabulary should be built
            Assert.NotEmpty(Idf); // Idf should be computed
        }

        [Fact]
        public void SetAverageDocumentLength_ShouldCalculateCorrectly()
        {
            // Arrange
            string[] documents = { "word1 word2", "word2 word3 word3" };
            Documents = documents;

            // Act
            SetAverageDocumentLength();

            // Assert
            Assert.Equal(2.5, GetAverageDocumentLength()); // Check average doc length
        }

        [Fact]
        public void ComputeVector_ShouldReturnCorrectBM25Values()
        {
            // Arrange
            string[] documents = { "word1 word2"};
            Fit(documents);

            var tokens = new List<string>
            {
                "word1", "word2"
            };

            // Act
            var bm25Vector = ComputeVector(tokens);

            // Assert
            Assert.True(bm25Vector.ContainsKey("word1"));
            Assert.True(bm25Vector.ContainsKey("word2"));
        }

        [Fact]
        public void ComputeDocFrequency_ShouldReturnCorrectFrequencies()
        {
            // Arrange
            string[] documents = { "word1 word2", "word2 word3" };
            Documents = documents;

            // Act
            var docFrequency = ComputeDocFrequency();

            // Assert
            Assert.Equal(1, docFrequency["word1"]); // word1 appears in 1 document
            Assert.Equal(2, docFrequency["word2"]); // word2 appears in 2 documents
            Assert.Equal(1, docFrequency["word3"]); // word3 appears in 1 document
        }

        [Fact]
        public void Tokenize_ShouldRemoveStopWords()
        {
            // Arrange
            string document = "the quick brown fox";

            // Act
            var tokens = Tokenize(document);

            // Assert
            Assert.DoesNotContain("the", tokens); // Stop word "the" should be removed
            Assert.Contains("quick", tokens);
            Assert.Contains("brown", tokens);
            Assert.Contains("fox", tokens);
        }

        [Fact]
        public void Fit_EmptyDocuments_ShouldNotThrowException()
        {
            // Arrange
            string[] documents = Array.Empty<string>();

            // Act
            Fit(documents);

            // Assert
            Assert.Empty(Vocabulary);
            Assert.Empty(Idf);
            Assert.Equal(0, GetAverageDocumentLength());
        }

        // ** Additional Tests for Indexer Methods **

        [Fact]
        public void BuildVocabulary_ShouldBuildCorrectVocabulary()
        {
            // Arrange
            string[] documents = { "apple banana", "banana orange" };

            // Act
            BuildVocabulary(documents);

            // Assert
            Assert.Equal(3, Vocabulary.Count); // "apple", "banana", and "orange" should be in the vocabulary
            Assert.True(Vocabulary.ContainsKey("apple"));
            Assert.True(Vocabulary.ContainsKey("banana"));
            Assert.True(Vocabulary.ContainsKey("orange"));
        }

        [Fact]
        public void Transform_ShouldReturnCorrectVectors()
        {
            // Arrange
            string[] documents = { "run walk jog", "walk run run" };

            // Act
            var vectors = Transform(documents);
            var vector1 = vectors[0];
            var vector2 = vectors[1];
            // Assert
            Assert.Equal(2, vectors.Count); // 2 documents, so 2 vectors
        }

        [Fact]
        public void FitTransform_ShouldReturnTransformedVectors()
        {
            // Arrange
            string[] documents = { "cat dog", "dog mouse" };

            // Act
            var vectors = FitTransform(documents);

            // Assert
            Assert.NotEmpty(Vocabulary); // Vocabulary should be built
            Assert.Equal(2, vectors.Count); // Should return vectors for both documents
        }

        [Fact]
        public void ComputeDocFrequency_ShouldHandleEmptyDocuments()
        {
            // Arrange
            string[] documents = Array.Empty<string>();
            Documents = documents;

            // Act
            var docFrequency = ComputeDocFrequency();

            // Assert
            Assert.Empty(docFrequency); // No documents, so no document frequencies
        }

        [Fact]
        public void Fit_ShouldHandleEmptyVocabularyGracefully()
        {
            // Arrange
            string[] documents = { "" };

            // Act
            Fit(documents);

            // Assert
            Assert.Empty(Vocabulary); // No words to build a vocabulary from
        }
    }
}





