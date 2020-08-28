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
        protected List<T> ItemsInRepository;
        protected Repository(IDataManager dataManager)
        {
            _dataManager = dataManager;
            ItemsInRepository = (List<T>)_dataManager.LoadAndDeserialize<T>() ?? new List<T>();
        }
        public void Save()
        {
            _dataManager.SaveToFile<T>(ItemsInRepository);
        }

        public virtual void Add(T item)
        {
            ItemsInRepository.Add(item);
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return ItemsInRepository.Where(predicate);
        }

        public T Get(string guid)
        {
            return SingleOrDefault(x => x.Id == guid );
        }

        public virtual void Remove(T item)
        {
            ItemsInRepository.Remove(item);
        }

        public T SingleOrDefault(Func<T, bool> predicate)
        {
            return ItemsInRepository.SingleOrDefault(predicate);
        }
        public IEnumerable<T> GetItems()
        {
            return ItemsInRepository;
        }
    }
}
