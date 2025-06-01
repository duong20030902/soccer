using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int ScheduleId { get; set; }

    public int UserId { get; set; }

    public DateTime BookingTime { get; set; }

    public decimal Price { get; set; }

    public decimal? Commission { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual FieldSchedule Schedule { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
