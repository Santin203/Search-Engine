using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Indexer.Tests
{
    public class FileHandlerTests
    {
        [Fact]
        public void StoreIndex_ValidData_ReturnsFilePath()
        {
            // Arrange
            var indexData = new Dictionary<string, Dictionary<string, double>>
            {
                { "doc1", new Dictionary<string, double> { { "term1", 1.0 }, { "term2", 2.0 } } },
                { "doc2", new Dictionary<string, double> { { "term1", 1.5 }, { "term2", 2.5 } } }
            };

            Indexer testIndexer = new Bm25();
            // Act
            string filePath = FileHandler.StoreIndex(indexData, testIndexer);

            // Assert
            Assert.True(File.Exists(filePath));
            Assert.Contains("Bm25",filePath);
            Assert.EndsWith(".json", filePath);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public void GetFilesFromFolder_ValidFolder_ReturnsFiles()
        {
            // Arrange
            var testFolder = Path.Combine(Directory.GetCurrentDirectory(), "test_files");
            Directory.CreateDirectory(testFolder);
            File.WriteAllText(Path.Combine(testFolder, "test1.txt"), "test content 1");
            File.WriteAllText(Path.Combine(testFolder, "test2.txt"), "test content 2");

            var fileHandler = new FileHandler();

            // Act
            var files = fileHandler.GetFilesFromFolder(testFolder);

            // Assert
            Assert.Contains(Path.Combine(testFolder, "test1.txt"), files);
            Assert.Contains(Path.Combine(testFolder, "test2.txt"), files);

            // Cleanup
            Directory.Delete(testFolder, true);
        }

        [Fact]
        public void ReadFileContent_ValidTxtFile_ReturnsContent()
        {
            // Arrange
            var fileHandler = new FileHandler();
            var testFile = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.WriteAllText(testFile, "test content");

            // Act
            string content = fileHandler.ReadFileContent(testFile);

            // Assert
            Assert.Equal("test content", content);

            // Cleanup
            File.Delete(testFile);
        }

        [Fact]
        public void ReadFileContent_UnsupportedFileType_ReturnsEmptyString()
        {
            // Arrange
            var fileHandler = new FileHandler();
            var testFile = Path.Combine(Directory.GetCurrentDirectory(), "unsupported.xyz");
            File.WriteAllText(testFile, "Unsupported content");

            // Act
            string content = fileHandler.ReadFileContent(testFile);

            // Assert
            Assert.Equal(string.Empty, content);

            // Cleanup
            File.Delete(testFile);
        }
    }
}
