using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal UnitCost { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public int? SizeId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductSize? ProductSize { get; set; }
}
