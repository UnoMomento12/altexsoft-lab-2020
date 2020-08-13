using System;
using System.IO;
using task1.TaskInterfaces;

namespace task1.FileTaskExecutors 
{
    abstract class AbstractFileExecutor : ITaskExecutor
    {
        protected CustomFileHandler cstFile;
        public TextHolder cstTextHolder;
        protected AbstractFileExecutor(TextHolder textHolder)
        {
            this.cstTextHolder = textHolder;
            this.cstFile = new CustomFileHandler(cstTextHolder);
           
        }
        public abstract void Execute(params string[] args);

       
    }
}
