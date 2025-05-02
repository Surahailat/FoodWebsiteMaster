using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public double? Height { get; set; }

    public double? Weight { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Paymentssub> Paymentssubs { get; set; } = new List<Paymentssub>();

    public virtual ICollection<UserSubscribe> UserSubscribes { get; set; } = new List<UserSubscribe>();
}
