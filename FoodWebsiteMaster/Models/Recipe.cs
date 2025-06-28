using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Recipe
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double? Rate { get; set; }

    public string? ServingSize { get; set; }

    public int? CaloriesTotal { get; set; }

    public double? Carbohydrate { get; set; }

    public double? TotalFat { get; set; }

    public double? Protein { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Direction> Directions { get; set; } = new List<Direction>();

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
}
