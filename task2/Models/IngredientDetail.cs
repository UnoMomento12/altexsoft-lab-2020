
namespace task2.Models
{
    class IngredientDetail
    {
        public Ingredient Ingredient { get; set; }
        public double Amount { get; set; }
        public IngredientDetail(Ingredient ing, double amount)
        {
            Ingredient = ing;
            Amount = amount;
        }
        
        
    }
}
