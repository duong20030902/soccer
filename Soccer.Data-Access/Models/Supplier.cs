using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? ContactInfo { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
