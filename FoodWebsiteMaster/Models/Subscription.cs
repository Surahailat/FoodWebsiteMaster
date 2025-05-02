using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationInDays { get; set; }

    public virtual ICollection<Paymentssub> Paymentssubs { get; set; } = new List<Paymentssub>();

    public virtual ICollection<UserSubscribe> UserSubscribes { get; set; } = new List<UserSubscribe>();
}
