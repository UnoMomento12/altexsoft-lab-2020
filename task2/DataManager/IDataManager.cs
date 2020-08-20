using System;
using System.Collections.Generic;
using System.Text;

namespace task2.DataManager
{
    interface IDataManager
    {
        void SaveToFile<T>(IEnumerable<T> ts) where T : class;
        IEnumerable<T> LoadAndDeserialize<T>() where T : class, new();
    }
}
