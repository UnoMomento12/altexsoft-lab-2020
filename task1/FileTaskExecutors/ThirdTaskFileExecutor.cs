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
            cstTextHolder.ClearTextHolder();
            cstFile.SetValidFilePath(args[0]);
            cstFile.ReadTextFromFile(); // Read text from file
            cstTextHolder.TextToSentences(); // Split text into sentences

            int sentenceId; 
            Int32.TryParse(args[1], out sentenceId);
            if (sentenceId <= 0 || sentenceId > cstTextHolder.Sentences.Count) { sentenceId = 3; } // set default value if parsed value is out of list bounds

            List<string> targetSentence = cstTextHolder.SentenceToWordList(sentenceId - 1);
            if (targetSentence.Count == 0)
            {
                Console.WriteLine("No sentence with such an id!\n");
                return;
            }
            Console.WriteLine("\nSentence number {0} with reversed letters in words: \n", sentenceId);
            for (int i = 0; i < targetSentence.Count; i++) // Display words of the sentence with reverse letters in the word
            {
                Console.Write("{0} ", ReverseWord(targetSentence[i]));
            }
            Console.WriteLine();
        }

        public string ReverseWord(string word)
        {
            char[] letters = word.ToCharArray();
            Array.Reverse(letters);
            return new string(letters);
        }
    }
}
