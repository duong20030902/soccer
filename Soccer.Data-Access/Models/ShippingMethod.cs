using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class ShippingMethod
{
    public int ShippingMethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public string? DeliveryTime { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
