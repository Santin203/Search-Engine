
namespace Indexer
{
    public class TxtFile : Files
    {
        public TxtFile()
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData;

            // Read all text from file
            fileData = File.ReadAllText(filePath);

            // Remove special chars from string and return
            fileData = RemoveBadChars(fileData);
            return fileData;
        }
    }
}