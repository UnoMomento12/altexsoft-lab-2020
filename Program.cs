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
        public static TextHolder txtHolder = new TextHolder();
        public static CustomFileHandler execOb = new CustomFileHandler(txtHolder);

        public Program()
        {

        }

        static void Main(string[] args)
        {
            Program start = new Program();
            while (true)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Please enter task function and arguments according to task1");
                    Console.WriteLine("Implemented functions: func1, func2, func3, func4");
                    Console.WriteLine("You can exit program by typing: exit ");
                    args = ParseArguments(Console.ReadLine());
                }
                if (args[0].Equals("exit")) break;
                start.ProcessArgs(args);
                args = new string[0];
            }
        }
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
            filePath = filePath.Replace("\"", "");
            Console.WriteLine(filePath);
            
            while (true)
            {
                
                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (String.IsNullOrEmpty(wordToDelete))
                    {
                        Console.WriteLine("Please enter a word/symbol for deletion:");
                        wordToDelete = Console.ReadLine();
                    }
                    string directory = fileInfo.DirectoryName;
                    string extension = fileInfo.Extension;
                    string fileName = Regex.Replace(fileInfo.Name, extension, ".orig" + extension);
                    string pathToCopy = directory + "\\" + fileName;

                    fileInfo.CopyTo(pathToCopy, true);

                    cstFile.ReadLinesFromFile(filePath);

                    int initialCount = txtHolder.CountTextLength();
                    int afterDelCount;

                    for (int i = 0; i < txtHolder.lines.Count; i++)
                    {
                        txtHolder.lines[i] = Regex.Replace(txtHolder.lines[i], wordToDelete, "");
                    }
                    afterDelCount = txtHolder.CountTextLength();

                    if (initialCount == afterDelCount)
                    {
                        Console.WriteLine("No matching entries to delete in text!");
                    }
                    cstFile.WriteLinesToFile(filePath);
                    break;
                } else {
                    Console.WriteLine("Please enter correct file path:");
                    filePath = Console.ReadLine().Replace("\"", "");
                }
            }
            
        }

        private void TaskFunction2(CustomFileHandler cstFile, string filePath)
        {
            filePath = filePath.Replace("\"", "");
            
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    
                    cstFile.ReadLinesFromFile(filePath);

                    txtHolder.LinesToWordArray();

                    Console.WriteLine("Quantity of words in text: {0} ", txtHolder.wordsInText.Count);

                    Console.WriteLine("Every 10th word in text:");

                    for (int i=9; i<txtHolder.wordsInText.Count; i = i+10)
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
            filePath = filePath.Replace("\"", "");
            
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    cstFile.ReadLinesFromFile(filePath);
                    txtHolder.LinesToSentences();
                    Console.WriteLine(txtHolder.sentences[2]);
                    string[] targetSentence = txtHolder.SentenceToWordArray(sentenceId);
                    Console.WriteLine("\n3rd sentence's words in reverse: \n");
                    for (int i = targetSentence.Length-1; i >= 0; i--)
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
                if (!String.IsNullOrEmpty(dirPath) && Directory.Exists(dirPath))
                {
                    cstDirSel = new CustomDirectorySelector(dirPath);
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter correct directory path!");
                    dirPath = Console.ReadLine().Replace("\"", "");
                }
            }
            
            while (true)
            {
                cstDirSel.ListDirectoriesAndFiles();
                Console.WriteLine("Enter id of selected directory, -1 to go to parent directory, ");
                Console.WriteLine("or lower number to exit this function:");
                int choice;
                if (Int32.TryParse(Console.ReadLine(),out choice))
                { 
                    if (choice < -1)
                    {
                        break;
                    }
                    cstDirSel.MoveToSelectedDir(choice);
                } else
                {
                    Console.WriteLine("Error occured while parsing your entry, please make your choise again:");
                    continue;
                }
            }
        }


    }

}
