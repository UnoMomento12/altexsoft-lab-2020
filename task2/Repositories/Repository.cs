using System;
using System.Linq;
using System.Collections.Generic;
using task2.DataManager;
using task2.Models;
namespace task2.Repositories
{
    abstract class Repository<T> : IRepository<T> where T: BaseModel, new()
    {
        private IDataManager _dataManager;
        protected List<T> _items;
        protected Repository(IDataManager dataManager)
        {
            _dataManager = dataManager;
            _items = (List<T>)_dataManager.LoadAndDeserialize<T>() ?? new List<T>();
        }
        public void Save()
        {
            _dataManager.SaveToFile<T>(_items);
        }

        public virtual void Add(T item)
        {
            _items.Add(item);
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _items.Where(predicate);
        }

        public T Get(string guid)
        {
            return SingleOrDefault(x => x.Id == guid );
        }

        public virtual void Remove(T item)
        {
            _items.Remove(item);
        }

        public T SingleOrDefault(Func<T, bool> predicate)
        {
            return _items.SingleOrDefault(predicate);
        }
        public IEnumerable<T> GetItems()
        {
            return _items;
        }
        
    }
}
