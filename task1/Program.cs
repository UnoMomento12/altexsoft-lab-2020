using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using task1.FileTaskExecutors;
using task1.TaskInterfaces;
using task1.DirectoryTaskExecutors;

namespace task1
{
    class Program
    {
        public ITaskExecutor taskExecutor;
        public TextHolder txtHolder;

        public Program()
        {
            txtHolder    = new TextHolder();
            taskExecutor = null;
        }

        static void Main(string[] args)
        {
            Program start = new Program();

            // Menu starts.
            while (true)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("\nPlease enter task function and arguments (file path in quotes) according to task1");
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
        /// Method <c>ProcessArgs</c> is used for processing arguments and executing corresponding functions specified in task1 of altex soft lab course.
        /// </summary>
        private void ProcessArgs(string[] args)
        {
            string path        = args.Length > 1 ? args[1] : "";
            string secondParam = args.Length > 2 ? args[2] : "";
            
            switch (args[0])
            {
                case "func1":
                    taskExecutor = new FirstTaskFileExecutor(txtHolder);
                    break;
                case "func2":
                    taskExecutor = new SecondTaskFileExecutor(txtHolder);
                    break;
                case "func3":
                    taskExecutor = new ThirdTaskFileExecutor(txtHolder);
                    break;
                case "func4":
                    taskExecutor = new FourthTaskDirectoryExecutor();
                    break;
                default:
                    Console.WriteLine("There is no such function!");
                    return;
            }
            taskExecutor.Execute(path, secondParam); // execute desired function
        }

    }

}
