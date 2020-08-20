using System;
using System.Collections.Generic;
using task2.Models;

namespace task2.Repositories
{
    interface IRepository<T> where T: BaseModel
    {
        void Add(T item);
        void Remove(T item);
        void Save();
        T Get(string guid);
        IEnumerable<T> GetItems();
        IEnumerable<T> Where(Func<T, bool> predicate);
        T SingleOrDefault(Func<T, bool> predicate);
       
    }
}
