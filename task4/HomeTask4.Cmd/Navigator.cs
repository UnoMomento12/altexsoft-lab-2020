using HomeTask4.Core.Entities;
using HomeTask4.Infrastructure.Data;
using HomeTask4.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
namespace HomeTask4.Cmd
{
    class Navigator
    {
        private IUnitOfWork _unitOfWork;
        private Category _root;
        private Category _current;
        private List<BaseEntity> _subItems;
        private int _recipesStart;
        

        public Navigator(IUnitOfWork unit)
        {
            _unitOfWork = unit;
        }
        public async void StartNavigator()
        {
            _subItems = new List<BaseEntity>();
             (await _unitOfWork.Categories.GetAllAsync()).Where(x => x.ParentId == null).ToList().ForEach(x => _subItems.Add(x));
            _recipesStart = _subItems.Count;
        }
        public async void MoveTo(int id)
        {
            bool inBounds = id > -1 && id < _subItems.Count;
            if (!inBounds)
            {
                Console.WriteLine("No category/recipe with such an id");
                return;
            }
            BaseEntity retrieved = _subItems[id];
            if(retrieved is Category check) //one cast like this?
            {
                _root = _current;
                _current = check;
                _subItems.Clear();
                (await _unitOfWork.Categories.GetAllAsync()).Where(x => x.Parent == _current).ToList().ForEach(x => _subItems.Add(x));
                _recipesStart = _subItems.Count;
                (await _unitOfWork.Recipes.GetAllAsync()).Where(x => x.CategoryId == _current.Id).ToList().ForEach(x => _subItems.Add(x));
            }    
            else  
            {
                WriteRecipe((Recipe) retrieved);
            }
        }

        public void WriteRecipe(Recipe recipe)
        { 
            Console.WriteLine("-------------------------------------------");
            PrintColored($"Recipe name: {recipe.Name}", ConsoleColor.Green);
            Console.WriteLine("Ingredients:");
            foreach (var a in recipe.Ingredients)
            {
                Console.WriteLine(a.Ingredient.Name + " : " + a.Amount + " " + a.Measure.Name);
            }
            Console.WriteLine("Steps to cook:");
            for(int i=0; i< recipe.Steps.OrderBy(x=>x.StepNumber).Count(); i++)
            {
                PrintColored( (i+1) + ". " + recipe.Steps[i] , ConsoleColor.Cyan);
            }
            PrintColored($"Description: {recipe.Description}" , ConsoleColor.Yellow);
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
                WriteRootNavigator();
            }
            else
            {
                WriteFullNavigator();
            }
        }

        private void WriteFullNavigator()
        {
            if (_subItems.Count == 0)
            {
                Console.WriteLine($"Category {_current.Name} is empty!");
                return;
            }
            Console.WriteLine($"Subcategories and recipes in {_current.Name} category.");
            WriteRootNavigator();
            Console.WriteLine("Recipes:");
            //for( int b = _recipesStart; b < _subItems.Count; b++) 
            //{
            //    Console.WriteLine("    "+ b +". "+ _subItems[b]?.Name);
            //}
            foreach( Recipe b in _subItems)
            {
                Console.WriteLine("    " + _subItems.IndexOf(b) + ". " + b?.Name);
            }
        }
        private void WriteRootNavigator()
        {
            Console.WriteLine("Categories:");
            //for (int i = 0; i < _recipesStart; i++)
            //{
            //    Console.WriteLine(i + ". " + _subItems[i].Name);
            //}
            foreach (Category c in _subItems)
            {
                Console.WriteLine("    " + _subItems.IndexOf(c) + ". " + c?.Name);
            }
        }
        

        public void GoBack()
        {
            _current = _root;
            _root = _current?.Parent;
            UpdateSubItems();
        }
        
        public Category GetCurrent()
        {
            return _current;
        }
        public Category GetCategory(int id)
        {
            bool inBounds = id > -1 && id < _recipesStart;
            if(inBounds)
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

        public async void UpdateSubItems()
        {
            _subItems.Clear();
            (await _unitOfWork.Categories.GetAllAsync()).Where(x => x.Parent == _current).ToList().ForEach(x => _subItems.Add(x));
            _recipesStart = _subItems.Count;
            if (_current != null)
            {
                (await _unitOfWork.Categories.GetAllAsync()).Where(x => x.ParentId == _current.Id).ToList().ForEach(x => _subItems.Add(x));
            }
        }

        public int GetSubItemsCount()
        {
            return _subItems.Count;
        }
    }
}
