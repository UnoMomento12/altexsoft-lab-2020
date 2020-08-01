using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using System.Text.RegularExpressions;

namespace task1
{
    class Program
    {
        public TextHolder txtHolder;
        public CustomFileHandler execOb;

        public Program()
        {
            txtHolder = new TextHolder();
            execOb = new CustomFileHandler(txtHolder);
        }

        static void Main(string[] args)
        {
            Program start = new Program();

            // Menu starts.
            while (true)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Please enter task function and arguments (file path in quotes) according to task1");
                    Console.WriteLine("Implemented functions: func1, func2, func3, func4");
                    Console.WriteLine("You can exit program by typing: exit ");
                    args = ParseArguments(Console.ReadLine());
                }
                if (args[0].Equals("exit")) break;
                start.ProcessArgs(args);
                args = new string[0];
            }
        }
        /// <summary>
        /// Method <c>ParseArguments</c> is used for transforming line of arguments into array of strings.
        /// </summary>
        static string[] ParseArguments(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray();
            bool inQuote = false;
            for (int index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"')
                    inQuote = !inQuote;
                if (!inQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split('\n');
        }

        /// <summary>
        /// Method <c>ProcessArgs</c> is used for processing arguments and executing corresponding functions cpecified in task1 of altex fost lab course.
        /// </summary>
        private void ProcessArgs(string[] args)
        {
            string path = null;
            switch (args[0])
            {
                case "func1":
                    path = args.Length > 1 ? args[1] : "";
                    string wordToDel = args.Length > 2 ? args[2] : "";
                    TaskFunction1(execOb, path, wordToDel);
                    break;
                case "func2":
                    path = args.Length > 1 ? args[1] : "";
                    TaskFunction2(execOb, path);
                    break;
                case "func3":
                    path = args.Length > 1 ? args[1] : "";
                    int id = 0;
                    id = (args.Length > 2 && Int32.TryParse(args[2], out id)) ? id : 2;
                    TaskFunction3(execOb, path, id);
                    break;
                case "func4":
                    path = args.Length > 1 ? args[1] : "";
                    TaskFunction4(path);
                    break;
                default:
                    Console.WriteLine("There is no such function!");
                    break;
            }
        }

       
        private void TaskFunction1(CustomFileHandler cstFile,  string filePath,  string wordToDelete)
        {
            filePath = filePath.Replace("\"", ""); // Deletes quotes in the string
            txtHolder.ClearTextHolder();
            while (true)
            {
                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath)) // If path directs to existing file, execute following:
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (String.IsNullOrEmpty(wordToDelete))  // Check the viability of the word for deletion
                    {
                        Console.WriteLine("Please enter a word/symbol for deletion:");
                        wordToDelete = Console.ReadLine();
                    }
                    string directory = fileInfo.DirectoryName; 
                    string extension = fileInfo.Extension;
                    string fileName = Regex.Replace(fileInfo.Name, extension, ".orig" + extension);
                    string pathToCopy = directory + "\\" + fileName;

                    fileInfo.CopyTo(pathToCopy, true); // Make copy of original file 

                    cstFile.ReadLinesFromFile(filePath); // Read lines of text from file

                    int initialCount = txtHolder.CountTextLength(); //Count quantity of words in the whole file
                    int afterDelCount;  // Variable for storing quantity of words after deleting

                    for (int i = 0; i < txtHolder.lines.Count; i++)
                    {
                        txtHolder.lines[i] = Regex.Replace(txtHolder.lines[i], wordToDelete, "");  //Replace symbol/word used for deletion with zero length string
                    }
                    afterDelCount = txtHolder.CountTextLength();

                    if (initialCount == afterDelCount)
                    {
                        Console.WriteLine("No matching entries to delete in text!"); // If lengths are equal, then there was no match in text for deleting
                    }
                    cstFile.WriteLinesToFile(filePath); //Write modified lines to file
                    break;
                } else {
                    Console.WriteLine("Please enter correct file path:");
                    filePath = Console.ReadLine().Replace("\"", "");
                }
            }
            
        }

        private void TaskFunction2(CustomFileHandler cstFile, string filePath)
        {
            filePath = filePath.Replace("\"", "");// Deletes quotes in the string
            txtHolder.ClearTextHolder();
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath)) // If path directs to existing file, execute following:
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    
                    cstFile.ReadLinesFromFile(filePath);

                    txtHolder.LinesToWordArray(); // Split lines into words

                    Console.WriteLine("Quantity of words in text: {0} ", txtHolder.wordsInText.Count); // Quantity of words

                    Console.WriteLine("Every 10th word in text:");

                    for (int i=9; i<txtHolder.wordsInText.Count; i = i+10) // Display every tenth word in text
                    {
                        Console.Write("{0} ", txtHolder.wordsInText[i]);
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

        private void TaskFunction3(CustomFileHandler cstFile, string filePath, int sentenceId = 2)
        {
            filePath = filePath.Replace("\"", ""); // Deletes quotes in the string
            txtHolder.ClearTextHolder();
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath)) // If path directs to existing file, execute following:
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    cstFile.ReadLinesFromFile(filePath); // Read lines from file
                    txtHolder.LinesToSentences(); // Split lines into sentences
                    List<string> targetSentence = txtHolder.SentenceToWordArray(sentenceId);
                    Console.WriteLine("\n3rd sentence's words in reverse: \n"); 
                    for (int i = targetSentence.Count-1; i >= 0; i--) // Display words of the sentence in reverse
                    {
                        Console.Write("{0} ", targetSentence[i]);
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

        private void TaskFunction4(string dirPath)
        {
            CustomDirectorySelector cstDirSel; 
            while (true)
            {
                if (!String.IsNullOrEmpty(dirPath) && Directory.Exists(dirPath)) // If path directs to existing directory:
                {
                    cstDirSel = new CustomDirectorySelector(dirPath); // Create directory selector
                    break; //exit cycle
                }
                else
                {
                    Console.WriteLine("Please enter correct directory path!"); // enter new path
                    dirPath = Console.ReadLine().Replace("\"", "");
                }
            }
            
            while (true)
            {
                cstDirSel.ListDirectoriesAndFiles(); //Display directories and files in the path
                Console.WriteLine("\nEnter id of selected directory, -1 to go to parent directory, ");
                Console.WriteLine("or lower number to exit this function:");
                int choice;
                if (Int32.TryParse(Console.ReadLine(),out choice))
                { 
                    if (choice < -1) // exit this function
                    {
                        break;
                    }
                    cstDirSel.MoveToSelectedDir(choice); // Change target directory
                } else
                {
                    Console.WriteLine("Error occured while parsing your entry, please make your choise again:");
                    continue;
                }
            }
        }


    }

}
