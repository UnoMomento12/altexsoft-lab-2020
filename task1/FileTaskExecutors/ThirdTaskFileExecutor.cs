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
            string filePath = CustomFileHandler.SetStringOrDefault(args[0], "");
            filePath = filePath.Replace("\"", ""); // Deletes quotes in the string
            int sentenceId;

            sentenceId = (!String.IsNullOrEmpty(args[1]) && Int32.TryParse(args[1], out sentenceId)) ? sentenceId : 3;
            cstTextHolder.ClearTextHolder();
            while (true)
            {

                if (File.Exists(filePath)) // If path directs to existing file, execute following:
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    cstFile.ReadTextFromFile(filePath); // Read lines from file
                    cstTextHolder.TextToSentences(); // Split lines into sentences
                    List<string> targetSentence = cstTextHolder.SentenceToWordList(sentenceId - 1);
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
