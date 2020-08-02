using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace task1.FileTaskExecutors
{
    class ThirdTaskFileExecutor : AbstractFileExecutor
    {
        public ThirdTaskFileExecutor(TextHolder textHolder) : base(textHolder) { }
        public override void Execute(params string[] args)
        {
            string filePath = !String.IsNullOrEmpty(args[0]) ? args[0].Replace("\"", "") : ""; // Deletes quotes in the string
            int sentenceId;

            sentenceId = (!String.IsNullOrEmpty(args[1]) && Int32.TryParse(args[1], out sentenceId)) ? sentenceId : 3;
            cstTextHolder.ClearTextHolder();
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath)) // If path directs to existing file, execute following:
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    cstFile.ReadLinesFromFile(filePath); // Read lines from file
                    cstTextHolder.LinesToSentences(); // Split lines into sentences
                    List<string> targetSentence = cstTextHolder.SentenceToWordArray(sentenceId - 1);
                    Console.WriteLine("\n{0} sentence's words with reversed letters: \n", sentenceId);
                    for (int i = 0; i < targetSentence.Count - 1; i++) // Display words of the sentence with reverse letters in the word
                    {
                        Console.Write("{0} ", ReverseWord(targetSentence[i]));
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

        public string ReverseWord(string word)
        {
            char[] letters = word.ToCharArray();
            Array.Reverse(letters);
            return new string(letters);
        }
    }
}
