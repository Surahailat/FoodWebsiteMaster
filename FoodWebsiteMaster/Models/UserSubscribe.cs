using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class UserSubscribe
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SubscriptionId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual Subscription Subscription { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
