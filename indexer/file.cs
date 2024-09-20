using System;
using System.IO;

namespace FilesSpace
{
    public class Files
    {
        public string fileData;
        public int nTerms;
        public (string term, int frequency)[] terms;

        public Files(string data, int termNumber, (string term, int frequency)[] termsList)
        {
            fileData = data;
            nTerms = termNumber;
            terms = termsList;
        }

        //Build instance of the Files Class
        public static Files ExtractContent()
        {
            //Attribute values to build class
            //Initialize to dummy values or leave as null
            string fileData;
            (string term, int frequency)[] terms = new (string, int)[1]{("",-1)};
            int nTerms = -1;

            Console.WriteLine("File build has started.");

            //Read data from file
            fileData = GetFileData();

            //File found, raw data stored
            if(fileData != null)
            {
                Console.WriteLine("File data read succesfully!");
                terms = ParseData(fileData);

                //Parsing of fileData succesfull
                if(terms[0].term != "")
                {
                    Console.WriteLine("File data parsed succesfully!");
                    nTerms = terms.Length;
                }
                //Error in parsing fileData
                else
                {
                    Console.WriteLine("File data could not be parsed.");
                }
            }
            //File not found
            else
            {
                Console.WriteLine("File data could not be read.");
            }
            return(new Files(fileData,nTerms,terms));
        }

        //Get file name from user, return data
        public static string GetFileData()
        {
            Console.WriteLine("Please write the name of the TXT file you want to read from: ");
            string fileName = Console.ReadLine();

            string fileData = null;

            if(File.Exists(fileName))
            {
                try
                {
                    fileData = File.ReadAllText(fileName);
                }
                catch(FileNotFoundException)
                {
                    Console.WriteLine($"File {fileName} not found in current directory.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"An I/O error occurred: {ex.Message}");
                }
            }
            return(fileData);
        }

        public static (string term, int frequency)[] ParseData(string rawData)
        {
            //Intialize tuple array to dummy value
            (string term, int frequency)[] terms = new (string, int)[1]{("",-1)};

            //Write code for stemming algorithm here.

            return(terms);
        }
    }

}