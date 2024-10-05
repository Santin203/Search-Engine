using System;
using System.Dynamic;
using System.IO;

using Porter2Stemmer;


namespace Indexer
{
    public abstract class Files
    {
        public string fileData;

        //Constructor for Files class, default values included
        public Files(string data = "")
        {
            fileData = data;
        }

        //Build instance of the Current Class
        public bool ExtractContent(string filePath)
        {
            Console.WriteLine("File build has started.");

            //Read data from file
            fileData = GetFileData(filePath);

            //File found, raw data stored
            if(!string.IsNullOrEmpty(fileData))
            {
                fileData = fileData.ToLower();
                Console.WriteLine("File data read successfully!");

                string[] words = fileData.Split(" ");
                words = StemWords(words);
                
                fileData = string.Join(" ", words);
                Console.WriteLine("Words have been stemmed");

                return true;
            }
            //File not found
            else
            {
                Console.WriteLine("File data could not be read.");
                return false;
            }
        }

        //Get file name from user, return data
        protected string GetFileData(string filePath)
        {

            string fileData = string.Empty;


            //Try to read file
            try
            {
                fileData = GetRawText(filePath);
            }
            //File was not found
            catch(FileNotFoundException)
            {
                Console.WriteLine($"File {filePath} not found.");
            }
            //IO error
            catch (IOException ex)
            {
                Console.WriteLine($"An I/O error occurred: {ex.Message}");
            }
            
            return fileData ?? string.Empty;
        }

        protected abstract string GetRawText(string filePath);
        
        protected string RemoveBadChars(string rawText)
        {
            string[] specialChars = { "@", "#", "!", "$", ",", ".", ";", "(", ")", "[", "]", "{", "}", "\"", "'", "\r", "\n" };

            // Replace each special character with an empty space
            foreach (string specialChar in specialChars)
            {
                rawText = rawText.Replace(specialChar, " ");
            }

            // Normalize spaces 
            rawText = System.Text.RegularExpressions.Regex.Replace(rawText, @"\s+", " ").Trim();
            
            return rawText;
        }
        protected string[] StemWords(string[] rawStrings)
        {
            // Make new instance of stemmer
            var stemmer = new EnglishPorter2Stemmer();

            // Traverse all strings in array
            for(int i = 0; i < rawStrings.Length; i++)
            {
                // Stem word in current i index
                var stemmed = stemmer.Stem(rawStrings[i]);

                // Assign stemmed word to i index
                rawStrings[i] = stemmed.Value;
            }
            return rawStrings;
        }
    }
}
