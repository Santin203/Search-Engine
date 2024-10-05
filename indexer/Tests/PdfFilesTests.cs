using System.IO;
using Xunit;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace Indexer.Tests
{
    public class PdfFileTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly PdfFile _pdfFile;

        public PdfFileTests()
        {
            // Arrange: Create a temporary PDF file for testing
            _testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "testFile.pdf");
            _pdfFile = new PdfFile();

            // Write sample PDF data to the test file
            PdfHelper.CreateSamplePdf(_testFilePath, "This is a test PDF content.");
        }

        [Fact]
        public void ExtractContent_PdfFileExists_ReturnsTrue()
        {
            // Act
            var result = _pdfFile.ExtractContent(_testFilePath);

            // Assert
            Assert.True(result);
            Assert.NotNull(_pdfFile.fileData);
            Assert.Contains("test pdf content", _pdfFile.fileData); // Check if content is extracted correctly
        }

        [Fact]
        public void ExtractContent_PdfFileDoesNotExist_ReturnsFalse()
        {
            // Act
            var result = _pdfFile.ExtractContent("non_existing_file.pdf");

            // Assert
            Assert.False(result);
            Assert.Empty(_pdfFile.fileData); // Check if fileData is empty when the file doesn't exist
        }

        [Fact]
        public void ExtractContent_PdfWithBadChars_ReturnsTrue()
        {
            // Arrange: Create a new PDF with special characters
            PdfHelper.CreateSamplePdf(_testFilePath, "Hello @#$,.:;. This PDF includes special chars!");

            // Act
            var result = _pdfFile.ExtractContent(_testFilePath);

            // Assert
            Assert.True(result);
            Assert.DoesNotContain("@", _pdfFile.fileData); // Ensure bad characters are removed
            Assert.Contains("hello", _pdfFile.fileData);  // Check that valid content remains
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

    public class PdfHelper
    {
        public static void CreateSamplePdf(string filePath, string content)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PdfSharp";

            // Create an empty page
            PdfPage page = document.AddPage();

            // Create an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create a font
            XFont font = new XFont("Verdana", 20);

            // Draw the text
            gfx.DrawString(content, font, XBrushes.Black,
                new XRect(0, 0, page.Width, page.Height),
                XStringFormats.TopLeft);

            // Save the document
            document.Save(filePath);
        }
    }
}

