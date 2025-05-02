using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? OrderStatus { get; set; }

    public DateTime? OrderDate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
