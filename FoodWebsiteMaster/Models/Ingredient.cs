using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Ingredient
{
    public int Id { get; set; }

    public int? RecipeId { get; set; }

    public string? IngredientText { get; set; }

    public virtual Recipe? Recipe { get; set; }
}
