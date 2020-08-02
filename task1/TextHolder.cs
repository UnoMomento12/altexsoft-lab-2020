using System;
using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;

namespace task1
{
    /// <summary>
    /// Class <c> TextHolder</c> is used for storing text from the file, transforming it
    /// as stated in test task's specification.
    /// </summary>
    class TextHolder
    {

        #region Lists which are needed for text deconstruction
        /// <summary>
        /// An array of lines in the text.
        /// </summary>
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
        /// Method <c>LinesToSentences</c> is used for transforming string lines into an array of strings (sentences),
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

        public void ClearTextHolder()
        {
            lines.Clear();
            sentences.Clear();
            wordsInText.Clear();
        }

        public List<string> SentenceToWordArray(int id)
        {

            if (id >= 0 && id < sentences.Count)
                return StringToWordList(sentences[id]);
            else return new List<string>();
        }

        /// <summary>
        /// Method <c>LinesToWordArray</c> is used for transforming lines into a sorted list of strings (words).
        /// Finally, the resulting collection is added to the word list.
        /// </summary>
        public void LinesToWordArray()
        {
            for (int i= 0; i < lines.Count;i++)
            {
                if (!String.IsNullOrEmpty(lines[i]))
                {
                    wordsInText.AddRange(StringToWordList(lines[i]));
                }
            }
        }

        public List<string> StringToWordList(string text)
        {
            char[] whitespace = new char[] { ' ', '\t', '\n' , ',' , '.' , '?', '!' };
            if (!String.IsNullOrEmpty(text))
                return Regex.Replace(text, "[^a-zA-Z0-9 ’,.?!]+", "").Split(whitespace, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            else return null;
        }

        public void DeleteWordFromLines(string wordToDelete)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = Regex.Replace(lines[i], wordToDelete, "");  //Replace symbol/word used for deletion with zero length string
            }
        }

        /// <summary>
        /// Counts quantity of words in the whole text
        /// </summary>
        /// <returns> quantity of words in the whole text</returns>
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
