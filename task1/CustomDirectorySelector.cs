using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace task1
{
    class CustomDirectorySelector
    {
        private List<string> _directoryNames;

        private List<string> _fileList;

        public string DirPath { get; set; }

        public CustomDirectorySelector()
        {
            DirPath = null;
            
            _directoryNames = new List<string>();
            
            _fileList = new List<string>();
        }

        private List<string> GetDirNames()
        {
            List<string> result = new List<string>();
            foreach (string fullName in Directory.GetDirectories(DirPath))
            {
                string name = new DirectoryInfo(fullName).Name;
                result.Add(name);
            }
            result.Sort();
            return result;
        }

        private List<string> GetFileNames()
        {
            List<string> result = new List<string>();
            foreach (string fullName in Directory.GetFiles(DirPath))
            {
                string name = new FileInfo(fullName).Name;
                result.Add(name);
            }
            result.Sort();
            return result;
        }


        public void MoveToSelectedDir(int id)
        {

            if (id == -1)
            {
                if (DirPath.Equals(Directory.GetDirectoryRoot(DirPath))) return;
                DirPath = Directory.GetParent(DirPath).FullName;
            }
            else if (_directoryNames.Count == 0 || _directoryNames.Count <= id || id < -1)
            {
                return;
            } 
            else
            {
                DirPath = DirPath + "\\" + _directoryNames[id];
            }
            _directoryNames.Clear();
            _directoryNames.AddRange(GetDirNames());
            _fileList.Clear();
            _fileList.AddRange(GetFileNames());
            
        }

        public void ListDirectoriesAndFiles()
        {
            Console.WriteLine("Directories:");
            Console.WriteLine("{0, 5} | {1,40}", "Id", "Name");

            for( int i = 0; i < _directoryNames.Count; i++)
            {
                Console.WriteLine("{0, 5} | {1,40}", i, _directoryNames[i]);
            }
            Console.WriteLine("Files:");
            Console.WriteLine("{0, 5} | {1,40}", "Id", "Name");
            for (int i = 0; i < _fileList.Count; i++)
            {
                Console.WriteLine("{0, 5} | {1,40}", i, _fileList[i] );
            }
        }

        public void SetValidDirectoryPath(string dirPath)
        {
            DirPath = dirPath.Replace("\"", ""); // get directory path

            while (true)
            {
                if (!Directory.Exists(DirPath)) // If path doesn't direct to existing directory:
                {
                    Console.WriteLine("Please enter correct directory path!"); // enter new path
                    DirPath = Console.ReadLine().Replace("\"", "");
                }
                else
                {
                    break; //exit cycle
                }
            }
            _directoryNames = GetDirNames();
            _fileList = GetFileNames();
        }
    }
}
