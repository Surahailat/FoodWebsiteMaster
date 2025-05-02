using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class NutritionFact
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? ServingSize { get; set; }

    public int? Calories { get; set; }

    public double? TotalFat { get; set; }

    public double? SaturatedFat { get; set; }

    public double? TransFat { get; set; }

    public double? Cholesterol { get; set; }

    public double? Sodium { get; set; }

    public double? TotalCarbohydrates { get; set; }

    public double? DietaryFiber { get; set; }

    public double? Sugars { get; set; }

    public double? Protein { get; set; }

    public double? VitaminD { get; set; }

    public double? Calcium { get; set; }

    public double? Iron { get; set; }

    public double? Potassium { get; set; }

    public virtual Product Product { get; set; } = null!;
}
