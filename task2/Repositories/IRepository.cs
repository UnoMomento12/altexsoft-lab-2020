using System;
using System.Collections.Generic;
using task2.Models;

namespace task2.Repositories
{
    interface IRepository<T> where T: BaseModel
    {
        void Add(T item);
        void Remove(T item);
        T GetById(string guid);
        IEnumerable<T> GetAll();
       
    }
}
