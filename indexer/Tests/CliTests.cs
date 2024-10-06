using System;
using System.IO;
using Xunit;

namespace Indexer.Tests
{
    public class CliTests : IDisposable
    {
        private readonly SearchEngineCore _searchEngine; 
        private readonly StringWriter _output;
        private readonly TextWriter _originalOut; // To restore original output

        public CliTests()
        {
            _searchEngine = new SearchEngineCore(new TFIDF(), new CosineSimilarity()); // Initialize the actual implementation
            _output = new StringWriter();
            _originalOut = Console.Out; // Save the original Console output
            Console.SetOut(_output); // Redirect Console output to StringWriter for assertions
        }

        [Fact]
        public void Run_IndexCommand_CallsIndexFolder()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("index\n1\n1\nC:\\Users\\jgabr\\Documents\\Code\\C#\\CS2365\\Project #1\\csharp-Search-Engine\\test_files\nexit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("Indexing completed.", _output.ToString());
        }

        [Fact]
        public void Run_SearchCommand_CallsSearch()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("search\nTest Query\n3\nexit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("\nPlease enter a command (index, search, load, or exit):", _output.ToString());
            Assert.Contains("Enter the search query:", _output.ToString());
            Assert.Contains("No results found.", _output.ToString()); // Assuming the search method has a way to return results
        }

        [Fact]
        public void Run_LoadCommand_CallsLoadIndex()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("load\nC:\\TestIndex.json\nexit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("Enter the path to the saved index file:", _output.ToString());
        }

        [Fact]
        public void Run_ExitCommand_ExitsGracefully()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("exit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("Exiting the search engine. Goodbye!", _output.ToString());
        }

        [Fact]
        public void Run_InvalidCommand_PrintsErrorMessage()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("invalid\nexit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("Invalid command. Please use 'index', 'search', 'load', or 'exit'.", _output.ToString());
        }

        [Fact]
        public void Run_EmptyFolderPath_PrintsErrorMessage()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("index\n1\n1\n\nexit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("Folder path cannot be empty.", _output.ToString());
        }

        [Fact]
        public void Run_SearchWithInvalidK_PrintsErrorMessage()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("search\nTest\ninvalid\nexit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("Please enter a valid number for k.", _output.ToString());
        }

        [Fact]
        public void Run_SearchWithEmptyQuery_PrintsErrorMessage()
        {
            // Arrange
            var cli = new Cli(_searchEngine);
            var inputs = new StringReader("search\n\n3\nexit\n");
            Console.SetIn(inputs); // Simulate user input

            // Act
            cli.Run();

            // Assert
            Assert.Contains("Query cannot be empty.", _output.ToString());
        }

        public void Dispose()
        {
            // Restore original Console output
            Console.SetOut(_originalOut); 
            _output.Dispose();
        }
    }
}

