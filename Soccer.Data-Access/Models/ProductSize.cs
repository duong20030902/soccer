using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class ProductSize
{
    public int ProductId { get; set; }

    public int SizeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual Size Size { get; set; } = null!;
}
