using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class ProductColor
{
    public int ProductId { get; set; }

    public int ColorId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Quantity { get; set; }

    public virtual Color Color { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
