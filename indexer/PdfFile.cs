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
            string pageData;

            string fileData = "";
            using (var pdf = PdfDocument.Open(filePath))
            {
                // Iterate through pages
                foreach (var page in pdf.GetPages())
                {
                    // Raw text of the page's content stream.
                    pageData = page.Text;

                    // Concatenate with previous data collected
                    fileData = string.Join(" ", fileData, pageData);
                }
            }

            // Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }
}