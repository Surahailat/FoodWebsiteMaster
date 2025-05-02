using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Paymentssub
{
    public int PaymentId { get; set; }

    public int UserId { get; set; }

    public int SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? CardLast4 { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual Subscription Subscription { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
