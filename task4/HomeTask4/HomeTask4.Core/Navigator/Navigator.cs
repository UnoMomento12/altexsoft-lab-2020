using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel;
using HomeTask4.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTask4.Core.Navigator
{
    public class Navigator
    {
        private IUnitOfWork _unitOfWork;
        private Category _root;
        public Category CurrentCategory { get; private set; }
        public int RecipesStart { get; private set; }
        public int ItemCount { get { return SubItems.Count; } }
        public List<BaseEntity> SubItems { get; }
        public Navigator(IUnitOfWork unit)
        {
            _unitOfWork = unit;
            SubItems = new List<BaseEntity>();
        }
        public async Task StartNavigator()
        {
             (await _unitOfWork.Repository.WhereAsync<Category>(x => x.ParentId == null)).ForEach(x => SubItems.Add(x));
            RecipesStart = SubItems.Count;
        }
        public bool InCategoriesBounds(int id)
        {
            return id > -1 && id < RecipesStart;
        }
        public async Task MoveToCategory(int id)
        {
            BaseEntity retrieved = SubItems[id];
            _root = CurrentCategory;
            CurrentCategory = retrieved as Category;
            await UpdateSubItems();
        }
        public bool InRecipesBounds(int id)
        {
            return id >= RecipesStart && id < SubItems.Count;
        }
        public List<BaseEntity> GetSubItems()
        {
            return SubItems;
        }

        public async Task GoBack()
        {
            CurrentCategory = _root;
            _root = CurrentCategory?.Parent;
            await UpdateSubItems();
        }
        
        public Category GetCurrent()
        {
            return CurrentCategory;
        }
        public Category GetCategory(int id)
        {
            bool inBounds = id > -1 && id < RecipesStart;
            if(inBounds)
            {
                return SubItems[id] as Category;
            }
            else
            {
                return null;
            }
        }
        public async Task UpdateSubItems()
        {
            SubItems.Clear();
            (await _unitOfWork.Repository.WhereAsync<Category>(x => x.Parent == CurrentCategory)).ForEach(x => SubItems.Add(x));
            RecipesStart = SubItems.Count;
            if (CurrentCategory != null)
            {
                (await _unitOfWork.Repository.WhereAsync<Recipe>(x => x.CategoryId == CurrentCategory.Id)).ForEach(x => SubItems.Add(x));
            }
        }
    }
}
