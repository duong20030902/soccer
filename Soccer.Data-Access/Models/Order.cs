using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public int ShippingMethodId { get; set; }

    public int AddressId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal TotalAmount { get; set; }

    public int StatusId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ShippingMethod ShippingMethod { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
