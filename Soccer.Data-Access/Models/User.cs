using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public decimal Balance { get; set; }

    public bool IsActive { get; set; }

    public string? EmailConfirmationToken { get; set; }

    public DateTime? ConfirmationTokenExpiry { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetTokenExpiry { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TopUpTransaction> TopUpTransactions { get; set; } = new List<TopUpTransaction>();
}
