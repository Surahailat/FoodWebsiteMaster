using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int? StockQuantity { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<NutritionFact> NutritionFacts { get; set; } = new List<NutritionFact>();
}
