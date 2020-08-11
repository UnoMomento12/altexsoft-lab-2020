using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace task1
{
    
    class CustomFileHandler
    {
        
        private TextHolder textHolder;

        public CustomFileHandler(TextHolder textHolder)
        {
            this.textHolder = textHolder;
        }

        public void ReadTextFromFile(string filePathToRead)
        {
            textHolder.TextInFile = File.ReadAllText(filePathToRead, Encoding.Default);
        }

        
        public void WriteTextToFile(string filePathToWrite)
        {
            File.WriteAllText(filePathToWrite, textHolder.TextInFile, Encoding.Default);
        }

        public static void SaveOriginalFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string directory = fileInfo.DirectoryName;
            string extension = fileInfo.Extension;
            string fileName = Regex.Replace(fileInfo.Name, extension, ".orig" + extension);
            string pathToCopy = directory + "\\" + fileName;

            fileInfo.CopyTo(pathToCopy, true); // Make copy of original file 
        }
        
        public static string SetStringOrDefault(string str, string defaultStr)
        {
            return !(str == null) ? str : defaultStr;
        }

    }
}
