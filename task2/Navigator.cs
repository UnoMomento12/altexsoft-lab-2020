using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using task2.Models;
using task2.UnitsOfWork;
namespace task2
{
    class Navigator
    {
        private UnitOfWork unitOfWork { get; set; }
        public Category Current { get; set; } 
        public List<Recipe> SubRecipes { get; set; }
        public List<Category> SubCategories { get; set; }
        

        public Navigator(UnitOfWork unit)
        {
            unitOfWork = unit;
            Current = null;
            SubCategories = unitOfWork.Categories.Where(x => x.ParentID == null).ToList();
            SubRecipes = null;
            Console.WriteLine(SubCategories.Count);
        }

        public void MoveToCategory(int id)
        {
            Category retr = SubCategories[id];
            Current = retr;
            SubCategories = unitOfWork.Categories.Where(x => x.ParentID == Current.ID).ToList();
            SubRecipes = Current.Recipes;
        }
        public void WriteNavigator()
        {
            Console.WriteLine("Navigator!");
            if (Current == null)
            {
                CurrentNUll();
            }
            else
            {
                CurrentNotNULL();
            }
        }

        public void CurrentNUll()
        {
            Console.WriteLine("Categories:");
            for( int i = 0; i< SubCategories?.Count; i++)
            {
                Console.WriteLine(i + ". " + SubCategories[i].Name);
            }
        }
        public void CurrentNotNULL()
        {
            Console.WriteLine("Categories:");
            for(int i = 0; i < SubCategories.Count; i++)
            {
                Console.WriteLine(i + ". " + SubCategories[i].Name);
                for( int b = 0; b < SubRecipes.Count; b++)
                {
                    Console.WriteLine("     " + b + ". " + SubRecipes[b].Name);
                }
            }
        }

        public void CloseAll()
        {
            
        }
    }
}
