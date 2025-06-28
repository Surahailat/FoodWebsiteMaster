using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Direction
{
    public int Id { get; set; }

    public int? RecipeId { get; set; }

    public int? StepNumber { get; set; }

    public string? DirectionText { get; set; }

    public virtual Recipe? Recipe { get; set; }
}
