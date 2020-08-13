using System;
using System.Collections.Generic;
using System.Text;

namespace task1.TaskInterfaces
{
    interface ITaskExecutor
    {
        void Execute(params string[] args) { }
    }
}
