using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace task1
{
    class CustomDirectorySelector
    {
        private string dirPath;
        
        private List<string> directoryNames;

        private List<string> fileList;

        public CustomDirectorySelector(string dirPath)
        {
            this.dirPath = dirPath;
            
            this.directoryNames = GetDirNames(dirPath);
            
            this.fileList = GetFileNames( dirPath);
        }

        private List<string> GetDirNames(string dirPath)
        {
            List<string> result = new List<string>();
            foreach (string fullName in Directory.GetDirectories(dirPath))
            {
                string name = new DirectoryInfo(fullName).Name;
                result.Add(name);
            }
            result.Sort();
            return result;
        }

        private List<string> GetFileNames(string dirPath)
        {
            List<string> result = new List<string>();
            foreach (string fullName in Directory.GetFiles(dirPath))
            {
                string name = new FileInfo(fullName).Name;
                result.Add(name);
            }
            result.Sort();
            return result;
        }


        public void MoveToSelectedDir(int id)
        {
            
            if( id == -1)
            {
                if (dirPath.Equals(Directory.GetDirectoryRoot(dirPath))) return;
                dirPath = Directory.GetParent(dirPath).FullName;
                directoryNames.Clear();
                directoryNames.AddRange(GetDirNames(dirPath));
                
            } else
            {
                if (directoryNames.Count == 0 || directoryNames.Count <= id || id < -1 ) return;
                dirPath = dirPath + "\\" + directoryNames[id];
                directoryNames.Clear();
                directoryNames.AddRange(GetDirNames(dirPath));
            }

            
            fileList.Clear();
            fileList.AddRange(GetFileNames(dirPath));
            
        }

        public void ListDirectoriesAndFiles()
        {
            Console.WriteLine("Directories:");
            Console.WriteLine("{0, 5} | {1,40}", "Id", "Name");

            for( int i = 0; i < directoryNames.Count; i++)
            {
                Console.WriteLine("{0, 5} | {1,40}", i, directoryNames[i]);
            }
            Console.WriteLine("Files:");
            Console.WriteLine("{0, 5} | {1,40}", "Id", "Name");
            for (int i = 0; i < fileList.Count; i++)
            {
                Console.WriteLine("{0, 5} | {1,40}", i, fileList[i] );
            }

        }



    }
}
