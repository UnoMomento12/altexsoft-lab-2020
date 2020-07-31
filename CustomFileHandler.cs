using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace task1
{
    /// <summary>
    /// Class <c> CustomFileHandler</c> is used for reading text from the file, transforming and writing it to the file,
    /// as stated in test task's specification.
    /// </summary>
    class CustomFileHandler
    {
        

        private TextHolder textHolder;

        /// <summary>
        /// Constructor <c>StringTransform</c> initializes object and sets base values.
        /// </summary>
        public CustomFileHandler(TextHolder textHolder)
        {
            
            
            this.textHolder = textHolder;
            
        }




        /// <summary>
        /// Method <c>ReadLinesFromFile</c> is used to read lines from the given file while skipping empty lines.
        /// Warning: there is high probability that this code won't read file properly if run in .NET Core framework,
        /// because .NET Core Encoding class recognizes smaller number of encodings
        /// Link : https://docs.microsoft.com/en-us/dotnet/api/system.text.encodinginfo.getencoding?view=netframework-4.8
        /// </summary>
        /// <param name="lines"></param>
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

        /// <summary>
        /// Method <c>WriteTextToFile</c> is used for writing transformed text to the file.
        /// </summary>
        /// <param name="writePath"></param>
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

        

    }

}
