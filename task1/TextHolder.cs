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
        /// String of text.
        /// </summary>
        public string TextInFile { get; set; }
        /// <summary>
        /// An array of Sentences in the text.
        /// </summary>
        public List<string> Sentences { get; set; }
        /// <summary>
        /// An array of words in the whole text.
        /// </summary>
        public List<string> WordsInText { get; set; }
        #endregion

        public TextHolder()
        {
            this.TextInFile = null;
            this.Sentences = new List<string>();
            this.WordsInText = new List<string>();
        }

        /// <summary>
        /// Method <c>TextToSentences</c> is used for transforming Text string into a list strings (Sentences),
        /// By using Regex engine text is split in cases where sequence of characters ends with sentence ending symbol or also a Quotation mark, 
        /// has white space after that AND the first char after white space is in the upper case.
        /// </summary>
        public void TextToSentences()
        {
            Sentences = new List<string>(Regex.Split(TextInFile, @"(?<=[\.!\?]|[\.!\?\”?])\s+(?=[A-Z])").Select(x => x.Trim()));
        }

        public void ClearTextHolder()
        {
            TextInFile = null;
            Sentences.Clear();
            WordsInText.Clear();
        }

        public List<string> SentenceToWordList(int id)
        {
            if (id >= 0 && id < Sentences.Count)
                return StringToWordList(Sentences[id]);
            else return new List<string>();
        }

        public void TextToWordList()
        {
            WordsInText = StringToWordList(TextInFile);
        }

        public List<string> StringToWordList(string text)
        {
            char[] whitespace = new char[] { ' ', '\t', '\n' , ',' , '.' , '?', '!' };
            if (!String.IsNullOrEmpty(text))
                return Regex.Replace(text, "[^a-zA-Z0-9 ’,.?!]+", "").Split(whitespace, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            else return null;
        }

        public void DeleteWordFromText(string wordToDelete)
        {
            TextInFile = Regex.Replace(TextInFile, wordToDelete, "");  //Replace symbol/word used for deletion with zero length string
        }

        
    }
}
