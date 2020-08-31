using System.Linq;
using System.Collections.Generic;
using task2.Models;
using task2.EntityList;
namespace task2.Repositories
{
    abstract class Repository<T> : IRepository<T> where T: BaseModel, new()
    {
        private Entities _entities;
        protected List<T> ItemsInRepository;
        protected Repository(Entities entities)
        {
            _entities = entities;
        }

        public virtual void Add(T item)
        {
            ItemsInRepository.Add(item);
        }


        public T GetById(string guid)
        {
            return ItemsInRepository.SingleOrDefault(x => x.Id == guid );
        }

        public virtual void Remove(T item)
        {
            ItemsInRepository.Remove(item);
        }

        public IEnumerable<T> GetAll()
        {
            return ItemsInRepository;
        }
    }
}
