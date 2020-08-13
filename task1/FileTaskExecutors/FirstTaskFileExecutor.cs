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
            cstTextHolder.ClearTextHolder();

            cstFile.SetValidFilePath(args[0]);

            string wordToDelete = args[1];
            
            while (true)
            {
                if (String.IsNullOrEmpty(wordToDelete))// Check the viability of the word for deletion
                {
                    Console.WriteLine("Please enter a word/symbol for deletion:");
                    wordToDelete = Console.ReadLine();
                } else
                {
                    break;
                }
            }

            cstFile.ReadTextFromFile(); // Read text from file
            cstFile.SaveOriginalFile(); // save original file with .orig suffix (no repeated reading)
            
            int initialCount = cstTextHolder.TextInFile.Length; //Count quantity of symbols in the whole file

            cstTextHolder.DeleteWordFromText(wordToDelete);

            int afterDelCount = cstTextHolder.TextInFile.Length;  // Count quantity of symbols after deleting

            if (initialCount == afterDelCount)
            {
                Console.WriteLine("\nNo matching entries to delete in text!"); // If lengths are equal, then there was no match in text for deleting
            } 
            else
            {
                cstFile.WriteTextToFile(); //Write modified text to file if there were changes
            }
            
        }
    }
}
