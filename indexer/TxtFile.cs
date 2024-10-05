
namespace Indexer
{
    public class TxtFile : Files
    {
        public TxtFile()
        {
        }

        // Constructor for TxtFiles that calls the base constructor
        public TxtFile(string data) 
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData;

            // Read all text from file
            fileData = File.ReadAllText(filePath);

            // Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }
}