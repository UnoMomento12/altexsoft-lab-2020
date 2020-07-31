using System;
using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;

namespace task1
{
    class TextHolder
    {

        #region Lists which are needed for text deconstruction
        public List<string> lines;
        /// <summary>
        /// An array of sentences in the text.
        /// </summary>
        public List<string> sentences;
        /// <summary>
        /// An array of words in the whole text.
        /// </summary>
        public List<string> wordsInText;
        #endregion

        public TextHolder()
        {
            this.lines = new List<string>();
            this.sentences = new List<string>();
            this.wordsInText = new List<string>();
        }

        /// <summary>
        /// Method <c>TextToTheSentences</c> is used for transforming text from file into an array of strings (sentences),
        /// By using Regex engine text is split in cases where sequence of characters ends with sentence ending symbol or also a Quotation mark, 
        /// has white space after that AND the first char after white space is in the upper case.
        /// </summary>
        public void LinesToSentences()
        {
            foreach (string line in lines)
            {
                if (!String.IsNullOrEmpty(line))
                    sentences.AddRange(Regex.Split(line, @"(?<=[\.!\?]|[\.!\?\”?])\s+(?=[A-Z])").Select(x => x.Trim()));
            }

        }

        public string[] SentenceToWordArray(int id)
        {
            if (id >= 0 && id < sentences.Count)
                return sentences[id].Split(' ');
            else return null;
        }

        /// <summary>
        /// Method <c>TextToWords</c> is used for transforming text into a sorted array of strings (words).
        /// Using Regex text engine every not needed character is removed from the text, except for white spaces and apostrophes.
        /// Then, text string is split on white spaces, removing empty strings and deleting white spaces around every word string.
        /// Finally, the resulting collection is sorted and cast to an array.
        /// </summary>
        public void LinesToWordArray()
        {
            char[] whitespace = new char[] { ' ', '\t', '\n' };
            foreach (string line in lines)
            {
                if (!String.IsNullOrEmpty(line))
                    wordsInText.AddRange(Regex.Replace(line, "[^a-zA-Z0-9 ’]+", "").Split(whitespace, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
            }

        }

        public int CountTextLength()
        {
            int count = 0;
            foreach (string line in lines)
            {
                count += line.Length;
            }
            return count;
        }
    }
}
