using System;
using System.Collections.Generic;
using System.Text;
using task1.TaskInterfaces;
using System.IO;
namespace task1.DirectoryTaskExecutors
{
    class FourthTaskDirectoryExecutor : ITaskExecutor
    {
        CustomDirectorySelector cstDirSel;

        public FourthTaskDirectoryExecutor()
        {
            this.cstDirSel = null;
        }

        public void Execute(params string[] args)
        {
            string dirPath = CustomFileHandler.SetStringOrDefault(args[0], ""); // get directory path
            dirPath = dirPath.Replace("\"", "");
            while (true)
            {
                if (Directory.Exists(dirPath)) // If path directs to existing directory:
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
                if (Int32.TryParse(Console.ReadLine(), out choice))
                {
                    if (choice < -1) // exit this function
                    {
                        break;
                    }
                    cstDirSel.MoveToSelectedDir(choice); // Change target directory
                }
                else
                {
                    Console.WriteLine("Error occured while parsing your entry, please make your choise again:");
                    continue;
                }
            }
        }
    }
}
