using System;
using HtmlAgilityPack;
using UglyToad.PdfPig;
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
        public Files(string data = "", int termNumber = -1, (string term, int frequency)[]? termsList = null)
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
            Console.WriteLine("Please write the name of the TXT file you want to read from: ");
            string fileName = Console.ReadLine() ?? string.Empty;

            string fileData = string.Empty;

            if(File.Exists(fileName))
            {
                //Try read file
                try
                {
                    fileData = File.ReadAllText(fileName);
                }
                //File was not found
                catch(FileNotFoundException)
                {
                    Console.WriteLine($"File {fileName} not found in current directory.");
                }
                //IO error
                catch (IOException ex)
                {
                    Console.WriteLine($"An I/O error occurred: {ex.Message}");
                }
            }
            return fileData ?? string.Empty;
        }
    }

    public class PdfFiles: Files
    {
        public PdfFiles(string data, int termNumber, (string term, int frequency)[] termsList)
        : base(data, termNumber, termsList)
        {
        }

        protected override string GetFileData()
        {
            Console.WriteLine("Please write the name of the PDF file you want to read from: ");
            string fileName = Console.ReadLine() ?? string.Empty;

            string fileData = string.Empty;
            string pageData = string.Empty;

            if(File.Exists(fileName))
            {
                //Try read file
                try
                {
                    fileData = "";
                    using (var pdf = PdfDocument.Open(fileName))
                    {
                        //Iterate through pages
                        foreach (var page in pdf.GetPages())
                        {
                            //raw text of the page's content stream.
                            pageData = page.Text;

                            //Concatinate with previous data collected
                            fileData = string.Join(" ",fileData, pageData);
                        }
                    }
                }
                //File was not found
                catch(FileNotFoundException)
                {
                    Console.WriteLine($"File {fileName} not found in current directory.");
                }
                //IO error
                catch (IOException ex)
                {
                    Console.WriteLine($"An I/O error occurred: {ex.Message}");
                }
            }
            return fileData ?? string.Empty;
        }
    }

        public class HTMLFiles: Files
    {
        public HTMLFiles(string data, int termNumber, (string term, int frequency)[] termsList)
        : base(data, termNumber, termsList)
        {
        }

        protected override string GetFileData()
        {
            Console.WriteLine("Please write the name of the HTML file you want to read from: ");
            string fileName = Console.ReadLine() ?? string.Empty;

            string fileData = string.Empty;

            if(File.Exists(fileName))
            {
                //Try read file
                try
                {
                    fileData = "";
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.Load(fileName);
                    fileData = htmlDoc.DocumentNode.InnerText;
                    
                }
                //File was not found
                catch(FileNotFoundException)
                {
                    Console.WriteLine($"File {fileName} not found in current directory.");
                }
                //IO error
                catch (IOException ex)
                {
                    Console.WriteLine($"An I/O error occurred: {ex.Message}");
                }
            }
            return fileData ?? string.Empty;
        }
    }

}