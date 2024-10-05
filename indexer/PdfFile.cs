using UglyToad.PdfPig;

namespace Indexer
{
    public class PdfFile : Files
    {
        public PdfFile()
        {
        }

        public PdfFile(string data)
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            // Check if the file exists before proceeding
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{filePath}' was not found.");
            }

            string fileData = "";
            using (var pdf = PdfDocument.Open(filePath))
            {
                // Iterate through pages
                foreach (var page in pdf.GetPages())
                {
                    // Concatenate with previous data collected
                    fileData = string.Join(" ", fileData, page.Text);
                }
            }

            // Remove special chars from string and return
            fileData = RemoveBadChars(fileData);
            return fileData;
        }
    }
}