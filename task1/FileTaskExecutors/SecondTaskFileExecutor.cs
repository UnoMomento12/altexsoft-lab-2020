using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace task1.FileTaskExecutors
{
    class SecondTaskFileExecutor : AbstractFileExecutor
    {
        public SecondTaskFileExecutor(TextHolder textHolder) : base(textHolder) { }

        public override void Execute(params string[] args)
        {
            string filePath = !String.IsNullOrEmpty(args[0]) ? args[0].Replace("\"", "") : "";// Deletes quotes in the string
            cstTextHolder.ClearTextHolder();
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath)) // If path directs to existing file, execute following:
                {
                    FileInfo fileInfo = new FileInfo(filePath);

                    cstFile.ReadLinesFromFile(filePath);

                    cstTextHolder.LinesToWordArray(); // Split lines into words

                    Console.WriteLine("Quantity of words in text: {0} \n", cstTextHolder.wordsInText.Count); // Quantity of words

                    Console.WriteLine("Every 10th word in text:");

                    for (int i = 9; i < cstTextHolder.wordsInText.Count; i = i + 10) // Display every tenth word in text
                    {
                        Console.Write("{0} ", cstTextHolder.wordsInText[i]);
                    }
                    Console.WriteLine();

                    break;
                }
                else
                {
                    Console.WriteLine("Please enter correct file path!");
                    filePath = Console.ReadLine().Replace("\"", "");
                }
            }
        }
    }
}
