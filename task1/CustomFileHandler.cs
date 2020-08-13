using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace task1
{
    
    class CustomFileHandler
    {
        
        private TextHolder textHolder;
        public string FilePath { get; set; }

        public CustomFileHandler(TextHolder textHolder)
        {
            this.textHolder = textHolder;
            FilePath = "";
        }

        public void ReadTextFromFile()
        {
            textHolder.TextInFile = File.ReadAllText(FilePath, Encoding.Default);
        }

        
        public void WriteTextToFile()
        {
            File.WriteAllText(FilePath, textHolder.TextInFile, Encoding.Default);
        }

        public void SaveOriginalFile()
        {
            FileInfo fileInfo = new FileInfo(FilePath);
            string directory = fileInfo.DirectoryName;
            string extension = fileInfo.Extension;
            string fileName = Regex.Replace(fileInfo.Name, extension, ".orig" + extension);
            string pathToCopy = directory + "\\" + fileName;
            File.WriteAllText(pathToCopy, textHolder.TextInFile, Encoding.Default); // Make copy of original file 
        }

        public void SetValidFilePath(string filePath)
        {
            FilePath = filePath.Replace("\"", ""); // Deletes quotation marks in the string
            while (true)
            {
                if (!File.Exists(FilePath)) // If path doesn't direct to existing file, execute following:
                {
                    Console.WriteLine("Please enter correct file path:");
                    FilePath = Console.ReadLine().Replace("\"", "");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
