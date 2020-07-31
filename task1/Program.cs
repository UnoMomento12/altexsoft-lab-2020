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



            //start.TaskFunction1(null, execOb);
            //start.TaskFunction2(null, execOb);
            //start.TaskFunction3(null, execOb);
            start.TaskFunction4(null);
            Console.ReadLine();
        }


        private void TaskFunction1(string filePath, CustomFileHandler cstFile)
        {

            ShowMenu();
            while (true)
            {
                
                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    Console.WriteLine("Please enter a word/symbol for deletion:");
                    string wordToDelete = "";
                    wordToDelete = Console.ReadLine();

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
                    Console.WriteLine("Please enter correct file path!");
                    filePath = Console.ReadLine().Replace("\"", "");
                }
            }
            
        }

        private void TaskFunction2(string filePath, CustomFileHandler cstFile)
        {
            ShowMenu();
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    
                    cstFile.ReadLinesFromFile(filePath);

                    txtHolder.LinesToWordArray();

                    Console.WriteLine("Quantity of words in text: {0} ", txtHolder.wordsInText.Count);

                    for (int i=9; i<txtHolder.wordsInText.Count; i = i+10)
                    {
                        Console.Write("{0} ", txtHolder.wordsInText[i]);
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter correct file path!");
                    filePath = Console.ReadLine().Replace("\"", "");
                }
            }
        }

        private void TaskFunction3(string filePath, CustomFileHandler cstFile , int sentenceId = 2)
        {
            ShowMenu();
            while (true)
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    cstFile.ReadLinesFromFile(filePath);
                    txtHolder.LinesToSentences();
                    Console.WriteLine(txtHolder.sentences[2]);
                    string[] targetSentence = txtHolder.SentenceToWordArray(sentenceId);
                    Console.WriteLine("3rd sentence's words in reverse:");
                    for (int i = targetSentence.Length-1; i >= 0; i--)
                    {
                        Console.Write("{0} ", targetSentence[i]);
                    }
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


        private void ShowMenu()
        {
            // Menu begins
            Console.WriteLine("Enter file name in the same folder as executable file;");
            Console.WriteLine("Or write full path including file name (Or drag and drop file on the console window).");
        }
    }

}
