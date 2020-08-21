using System;
using System.Collections.Generic;
using System.Linq;
using task2.Models;
using task2.UnitsOfWork;
namespace task2
{
    class Navigator
    {
        private UnitOfWork _unitOfWork;
        private Category _root;
        private Category _current;
        private List<BaseModel> _subItems;

        private int _recipesStart { get; set; }
        

        public Navigator(UnitOfWork unit)
        {
            _unitOfWork = unit;
            _current = null;
            _root = null;
            _subItems = new List<BaseModel>();
            _unitOfWork.Categories.Where(x => x.ParentID == null).ToList().ForEach(x=> _subItems.Add(x));
            _recipesStart = _subItems.Count;
        }

        public void MoveTo(int id)
        {
            bool inBounds = id > -1 && id < _subItems.Count;
            if (!inBounds)
            {
                Console.WriteLine("No category/recipe with such an id");
                return;
            }
            BaseModel retr = _subItems[id];
            if (retr is Category)
            {
                _root = _current;
                _current = retr as Category;
                _subItems.Clear();
                _unitOfWork.Categories.Where(x => x.Parent == _current).ToList().ForEach(x => _subItems.Add(x));
                _recipesStart = _subItems.Count;
                _current.Recipes.ForEach(x => _subItems.Add(x));
            }
            else if ( retr is Recipe) 
            {
                WriteRecipe(retr as Recipe);
            }
        }

        public void WriteRecipe(Recipe recipe)
        { 
            Console.WriteLine("-------------------------------------------");
            PrintColored("Recipe name: " + recipe.Name , ConsoleColor.Green);
            Console.WriteLine("Ingredients:");
            foreach (var a in recipe.Ingredients)
            {
                Console.WriteLine(a.Ingredient.Name + " : " + a.Amount + " " + a.Ingredient.Denomination);
            }
            Console.WriteLine("Steps to cook:");
            for(int i=0; i< recipe.Steps.Count; i++)
            {
                PrintColored( (i+1) + ". " + recipe.Steps[i] , ConsoleColor.Cyan);
            }
            PrintColored("Description: " + recipe.Description , ConsoleColor.Yellow);
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Press any button to close recipe!");
            Console.ReadKey();
        }
        
        public void WriteNavigator()
        {
            Console.Clear();
            Console.WriteLine("Navigator!");
            if (_current == null)
            {
                WriteNavNull();
            }
            else
            {
                WriteNav();
            }
        }

        private void WriteNav()
        {
            if (_subItems.Count == 0)
            {
                Console.WriteLine("Category " + _current.Name + " is empty!");
                return;
            }
            Console.WriteLine("Subcategories and recipes in " + _current.Name + " category.");
            Console.WriteLine("Categories:");
            int i;
            for(i = 0; i< _recipesStart; i++)
            {
                Console.WriteLine(i + ". " + _subItems[i].Name);
            }
            Console.WriteLine("Recipes:");
            for( int b = i; b < _subItems?.Count; b++) 
            {
                Console.WriteLine("    "+ b +". "+ _subItems[b].Name);
            }

        }
        private void WriteNavNull()
        {
            Console.WriteLine("Categories:");
            for (int i = 0; i < _subItems?.Count; i++)
            {
                Console.WriteLine(i + ". " + _subItems[i].Name);
            }
        }
        

        public void GoBack()
        {
            _current = _root;
            _root = _current?.Parent;
            _subItems.Clear();
            _unitOfWork.Categories.Where(x => x.Parent == _current).ToList().ForEach(x => _subItems.Add(x));
            _recipesStart = _subItems.Count;
            _current?.Recipes.ForEach(x => _subItems.Add(x));
        }
        
        public Category GetCurrent()
        {
            return _current;
        }
        public Category GetCategory(int id)
        {
            bool inBounds = id > -1 && id < _subItems.Count;
            if(inBounds && _subItems[id] is Category )
            {
                return _subItems[id] as Category;
            }
            else
            {
                return null;
            }
        }

        private void PrintColored(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void UpdateSubItems()
        {
            _subItems.Clear();
            _unitOfWork.Categories.Where(x => x.Parent == _current).ToList().ForEach(x => _subItems.Add(x));
            _recipesStart = _subItems.Count;
            _current?.Recipes.ForEach(x => _subItems.Add(x));
        }
    }
}
