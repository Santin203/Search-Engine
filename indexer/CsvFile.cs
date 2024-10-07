namespace Indexer
{
    public class CSVFile : Files
    {
        public CSVFile()
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData;

            fileData = File.ReadAllText(filePath);
            fileData = RemoveBadChars(fileData);

            return fileData;
        }
    }
}