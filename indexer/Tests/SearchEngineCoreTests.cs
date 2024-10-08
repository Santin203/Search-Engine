using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Indexer.Tests
{
    public class SearchEngineCoreTests
    {
        private readonly Indexer _mockIndexer;
        private readonly SearchEngine _mockDistance;

        public SearchEngineCoreTests()
        {
            // Create mock instances of Indexer and SearchEngine
            _mockIndexer = new TFIDF();
            _mockDistance = new CosineSimilarity();
        }

        [Fact]
        public void SetVectorizer_ShouldSetCorrectVectorizer()
        {
            // Arrange
            var searchEngineCore = new SearchEngineCore(_mockIndexer, _mockDistance);

            // Act
            searchEngineCore.SetVectorizer(2); // Set to Bm25

            Indexer testVectorizer = searchEngineCore.GetVectorizer();

            // Assert
            Assert.IsType<Bm25>(testVectorizer); // Check if the vectorizer is Bm25
        }

        [Fact]
        public void SetDistance_ShouldSetCorrectDistance()
        {
            // Arrange
            var searchEngineCore = new SearchEngineCore(_mockIndexer, _mockDistance);

            // Act
            searchEngineCore.SetDistance(2); // Set to Searchito

            SearchEngine testDistance = searchEngineCore.GetDistance();

            // Assert
            Assert.IsType<Searchito>(testDistance); // Check if the distance is Searchito
        }

        [Fact]
        public void Search_ShouldReturnResults_WhenValidQueryIsProvided()
        {
            // Arrange
            var searchEngineCore = new SearchEngineCore(_mockIndexer, _mockDistance);

            // Load premade index
            string folderPath = "C:\\Users\\jgabr\\Documents\\Code\\C#\\CS2365\\Project #1\\csharp-Search-Engine\\indexer\\Tests\\Bm2586.json";
            searchEngineCore.LoadIndex(folderPath);
            string query = "test";

            // Act
            var results = searchEngineCore.Search(query, 5); // Request top 5 results

            // Assert
            Assert.NotEmpty(results); // Ensure that results are returned
        }

        [Fact]
        public void Search_ShouldReturnEmptyList_WhenQueryVectorIsEmpty()
        {
            // Arrange
            var searchEngineCore = new SearchEngineCore(_mockIndexer, _mockDistance);
            searchEngineCore.IndexFolder("C:\\Users\\jgabr\\Documents\\Code\\C#\\CS2365\\Project #1\\csharp-Search-Engine\\indexer\\Tests\\test_files");
            string emptyQuery = ""; // Empty query

            // Act
            var results = searchEngineCore.Search(emptyQuery, 5); // Request top 5 results

            // Assert
            Assert.Empty(results); // Ensure that an empty list is returned
        }
    }

}
