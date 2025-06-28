using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? TransactionId { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? City { get; set; }

    public string? ApartmentNumber { get; set; }

    public string? NameOnCard { get; set; }

    public string? CardNumber { get; set; }

    public string? ExpiryCard { get; set; }

    public int? Cvv { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
