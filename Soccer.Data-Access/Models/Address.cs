using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int UserId { get; set; }

    public string RecipientName { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string CityProvince { get; set; } = null!;

    public string? PostalCode { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
