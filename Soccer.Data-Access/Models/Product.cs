using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public int SupplierId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SalePrice { get; set; }

    public int StockQuantity { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    public virtual Supplier Supplier { get; set; } = null!;
}
