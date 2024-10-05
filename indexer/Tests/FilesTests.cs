using System.IO;
using Xunit;

namespace Indexer.Tests
{
    public class FilesTests : IDisposable
    {
        private readonly List<Files> _filesInstances;
        private readonly List<string> _testFilePaths;
        private readonly List<string> _testFileContents;

        public FilesTests()
        {
            // Initialize the list of Files subclasses
            _filesInstances = new List<Files>
            {
                new CSVFile(),
                new TxtFile(),
                new HTMLFile(),
                new XmlFile(),
                new JsonFile()
            };

            // Initialize corresponding file names and content
            _testFilePaths = new List<string>
            {
                Path.Combine(Directory.GetCurrentDirectory(), "testFile.csv"),
                Path.Combine(Directory.GetCurrentDirectory(), "testFile.txt"),
                Path.Combine(Directory.GetCurrentDirectory(), "testFile.html"),
                Path.Combine(Directory.GetCurrentDirectory(), "testFile.xml"),
                Path.Combine(Directory.GetCurrentDirectory(), "testFile.json")
            };

            _testFileContents = new List<string>
            {
                "name,age\n@John,30\nDoe,25",
                "Hello World! This is a test text file @John.",
                "<html><body>Hello @John HTML!</body></html>",
                "<John><element>Test XML @</element></John>",
                "{\"name\": \"@John\", \"age\": 30}"
            };

            // Create test files with corresponding content
            for (int i = 0; i < _testFilePaths.Count; i++)
            {
                File.WriteAllText(_testFilePaths[i], _testFileContents[i]);
            }
        }

        [Fact]
        public void ExtractContent_FileExists_ReturnsTrue()
        {
            for (int i = 0; i < _filesInstances.Count; i++)
            {
                // Act
                var result = _filesInstances[i].ExtractContent(_testFilePaths[i]);

                // Assert
                Assert.True(result);
                Assert.NotNull(_filesInstances[i].fileData);
            }
        }

        [Fact]
        public void ExtractContent_FileDoesNotExist_ReturnsFalse()
        {
            for (int i = 0; i < _filesInstances.Count; i++)
            {
                // Act
                var result = _filesInstances[i].ExtractContent("non_existing_file.txt");

                // Assert
                Assert.False(result);
                Assert.Empty(_filesInstances[i].fileData);
            }
        }

        [Fact]
        public void ExtractContent_FileWithBadChars_ReturnsTrue()
        {
            for (int i = 0; i < _filesInstances.Count; i++)
            {
                // Act
                var result = _filesInstances[i].ExtractContent(_testFilePaths[i]);

                // Assert
                Assert.True(result);
                Assert.Contains("john", _filesInstances[i].fileData);  
                Assert.DoesNotContain("@", _filesInstances[i].fileData); 
            }
        }

        public void Dispose()
        {
            // Cleanup: Delete the test files if they exist
            foreach (var filePath in _testFilePaths)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
