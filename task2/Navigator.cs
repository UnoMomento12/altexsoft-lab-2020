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
        public Category Root { get; set; }
        public Category Current { get; set; } 
        public List<BaseModel> SubItems { get; set; }
        

        public Navigator(UnitOfWork unit)
        {
            unitOfWork = unit;
            Current = null;
            Root = null;
            SubItems = new List<BaseModel>();
            unitOfWork.Categories.Where(x => x.ParentID == null).ToList().ForEach(x=> SubItems.Add(x));
        }

        public void MoveTo(int id)
        {
            BaseModel retr = SubItems[id];
            if (retr is Category)
            {
                Current = retr as Category;
                Root = Current.Parent;
                SubItems.Clear();
                unitOfWork.Categories.Where(x => x.ParentID == Current.ID).ToList().ForEach(x => SubItems.Add(x));
                Current.Recipes.ForEach(x => SubItems.Add(x));
            }
            else if ( retr is Recipe) 
            {
                WriteRecipe(retr as Recipe);
            }
        }

        public void WriteRecipe(Recipe recipe)
        {
            Console.Clear();
            Console.WriteLine("Recipe name: " + recipe.Name);
            Console.WriteLine("Ingredients:");
            foreach (var a in recipe.Ingredients)
            {
                Console.WriteLine(a.Ingredient.Name + " : " + a.Amount);
            }
            Console.WriteLine("Steps to cook:");
            foreach(var a in recipe.Steps)
            {
                Console.WriteLine(a);
            }
            Console.WriteLine("Description: " + recipe.Description);
        }
        
        public void WriteNavigator()
        {
            Console.Clear();
            Console.WriteLine("Navigator!");
            
            if (Current == null)
            {
                WriteNav();
            }
            else
            {
                Console.WriteLine("Subcategories and recipes in " + Current.Name + " category:");
                WriteNav();
            }
        }

        public void WriteNav()
        {
            Console.WriteLine("Categories:");
            for( int i = 0; i< SubItems?.Count; i++)
            {
                Console.WriteLine(i + ". " + SubItems[i].Name);
            }
        }
        

        public void GoBack()
        {

        }
        public void ShowAllCategories()
        {

        }
    }
}
