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
            cstTextHolder.ClearTextHolder();
            cstFile.SetValidFilePath(args[0]);
            cstFile.ReadTextFromFile();

            cstTextHolder.TextToWordList(); // Split text into words

            Console.WriteLine("Quantity of words in text: {0} \n", cstTextHolder.WordsInText.Count); // Quantity of words

            Console.WriteLine("Every 10th word in text:");

            for (int i = 9; i < cstTextHolder.WordsInText.Count; i = i + 10) // Display every tenth word in text
            {
                Console.Write("{0} ", cstTextHolder.WordsInText[i]);
            }
            Console.WriteLine();
        }
    }
}
