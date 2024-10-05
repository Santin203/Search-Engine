using System.IO;
using Xunit;

namespace Indexer.Tests
{
    public class FilesTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly Files _files;

        public FilesTests()
        {
            // Arrange: Create a temporary file for testing
            _testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "testFile.txt");
            _files = new TestFiles(); // Replace TestFiles with your implementation of Files

            // Write sample data to the test file
            File.WriteAllText(_testFilePath, "Hello World! This is a test file running !@#$,.:;.");
        }

        [Fact]
        public void ExtractContent_FileExists_ReturnsTrue()
        {
            // Act
            var result = _files.ExtractContent(_testFilePath);

            // Assert
            Assert.True(result);
            Assert.NotNull(_files.fileData);
        }

        [Fact]
        public void ExtractContent_FileDoesNotExist_ReturnsFalse()
        {
            // Act
            var result = _files.ExtractContent("non_existing_file.txt");

            // Assert
            Assert.False(result);
            Assert.Empty(_files.fileData);
        }

        [Fact]
        public void ExtractContent_FileWithBadChars_ReturnsTrue()
        {
            // Act
            var result = _files.ExtractContent(_testFilePath);

            // Assert
            Assert.True(result);
            Assert.Contains("run", _files.fileData);    // Check words are getting stemmed
            Assert.DoesNotContain("@",_files.fileData); // Check that bad chars are getting removed
        }

        public void Dispose()
        {
            // Cleanup: Delete the test file if it exists
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }

    // A concrete implementation of the abstract Files class for testing purposes
    public class TestFiles : Files
    {
        protected override string GetRawText(string filePath)
        {
            string data = File.ReadAllText(filePath);
            data = RemoveBadChars(data);
            return data;

        }
    }
}
