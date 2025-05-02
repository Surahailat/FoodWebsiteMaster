using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? AddedAt { get; set; }

    public int UserId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public decimal Price { get; set; }

    public string? Image { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
