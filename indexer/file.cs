using System;
using System.Dynamic;
using System.IO;

namespace FilesSpace
{
    public abstract class Files
    {
        public string fileData;
        public int nTerms;
        public (string term, int frequency)[] terms;

        //Constructor for Files class, default values included
        public Files(string data = "", int termNumber = -1, (string term, int frequency)[] termsList = null)
        {
            fileData = data;
            nTerms = termNumber;
            terms = termsList ?? new (string, int)[1] { ("", -1) };  // Initialize if termsList is null
        }

        //Build instance of the Current Class
        public bool ExtractContent()
        {
            Console.WriteLine("File build has started.");

            //Read data from file
            this.fileData = this.GetFileData();

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
                    return true;
                }
                //Error in parsing fileData
                else
                {
                    Console.WriteLine("File data could not be parsed.");
                    return false;
                }
            }
            //File not found
            else
            {
                Console.WriteLine("File data could not be read.");
                return false;
            }
        }

        //Get file name from user, return data
        protected abstract string GetFileData();
        
        protected static (string term, int frequency)[] ParseData(string rawData)
        {
            //Intialize tuple array to dummy value
            (string term, int frequency)[] terms = new (string, int)[1]{("",-1)};

            //Write code for stemming algorithm here.

            return(terms);
        }
    }

    public class TxtFiles: Files
    {
        // Constructor for PdfFiles that calls the base constructor
        public TxtFiles(string data, int termNumber, (string term, int frequency)[] termsList)
        : base(data, termNumber, termsList)
        {
        }

        protected override string GetFileData()
        {
            Console.WriteLine("Please write the name of the file you want to read from: ");
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
    }

}