using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<TopUpTransaction> TopUpTransactions { get; set; } = new List<TopUpTransaction>();
}
