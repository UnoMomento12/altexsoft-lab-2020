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

        public void ReadLinesFromFile(string filePathToRead)
        {
            string currentLine = null;
            try
            {
                using (StreamReader sr = new StreamReader(filePathToRead, Encoding.Default))
                {
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        //if (!String.IsNullOrEmpty(currentLine))
                            textHolder.lines.Add(currentLine);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        
        public void WriteLinesToFile(string filePathToWrite)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathToWrite, false, Encoding.Default))
                {
                    foreach(string line in textHolder.lines)
                    {
                        sw.WriteLine(line);     
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
    }
}
