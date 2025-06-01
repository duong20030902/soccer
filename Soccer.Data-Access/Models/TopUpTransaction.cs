using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class TopUpTransaction
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionTime { get; set; }

    public string Status { get; set; } = null!;

    public string? MoMoTransId { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
