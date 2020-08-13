using System;
using System.Collections.Generic;
using System.Text;
using task1.TaskInterfaces;
using System.IO;
namespace task1.DirectoryTaskExecutors
{
    class FourthTaskDirectoryExecutor : ITaskExecutor
    {
        private CustomDirectorySelector cstDirSel;

        public FourthTaskDirectoryExecutor()
        {
            this.cstDirSel = new CustomDirectorySelector();
        }

        public void Execute(params string[] args)
        {
            cstDirSel.SetValidDirectoryPath(args[0]);
            
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
