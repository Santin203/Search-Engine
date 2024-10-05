using HtmlAgilityPack;

namespace Indexer
{
    public class HTMLFile : Files
    {
        public HTMLFile()
        {
        }

        public HTMLFile(string data)
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            // Load the HTML file
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(filePath);

            // Extract the plain text content from the HTML
            string fileData = htmlDoc.DocumentNode.InnerText;

            // Remove unwanted special characters from the text
            string cleanedText = RemoveBadChars(fileData);

            // Return the cleaned plain text content
            return cleanedText;
        }
    }

}