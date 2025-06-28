namespace FoodWebsiteMaster.Models
{
    public class RecipeDetailsViewModel
    {
            public Recipe Recipe { get; set; }
            public List<Direction> Directions { get; set; }
            public List<Ingredient> Ingredients { get; set; }
    }
}
