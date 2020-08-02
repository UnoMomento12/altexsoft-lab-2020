using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace task1.FileTaskExecutors
{
    class FirstTaskFileExecutor : AbstractFileExecutor
    {
        public FirstTaskFileExecutor(TextHolder textHolder) : base(textHolder) { }

        public override void Execute(params string[] args)
        {
            string filePath = !String.IsNullOrEmpty(args[0]) ? args[0] : "";
            string wordToDelete = !String.IsNullOrEmpty(args[1]) ? args[1] : "";
            filePath = filePath.Replace("\"", ""); // Deletes quotes in the string
           
            cstTextHolder.ClearTextHolder();
            while (true)
            {
                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath)) // If path directs to existing file, execute following:
                {
                    if (String.IsNullOrEmpty(wordToDelete))  // Check the viability of the word for deletion
                    {
                        Console.WriteLine("Please enter a word/symbol for deletion:");
                        wordToDelete = Console.ReadLine();
                    }

                    CustomFileHandler.SaveOriginalFile(filePath);// save original file with .orig suffix
                    
                    cstFile.ReadLinesFromFile(filePath); // Read lines of text from file

                    int initialCount = cstTextHolder.CountTextLength(); //Count quantity of words in the whole file
                    int afterDelCount;  // Variable for storing quantity of words after deleting

                    cstTextHolder.DeleteWordFromLines(wordToDelete);

                    afterDelCount = cstTextHolder.CountTextLength();

                    if (initialCount == afterDelCount)
                    {
                        Console.WriteLine("\nNo matching entries to delete in text!"); // If lengths are equal, then there was no match in text for deleting
                    }
                    cstFile.WriteLinesToFile(filePath); //Write modified lines to file
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter correct file path:");
                    filePath = Console.ReadLine().Replace("\"", "");
                }
            }
        }
    }
}
