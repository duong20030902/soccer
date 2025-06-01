using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class FieldSchedule
{
    public int ScheduleId { get; set; }

    public int FieldId { get; set; }

    public DateOnly Date { get; set; }

    public int TimeslotId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Field Field { get; set; } = null!;

    public virtual Timeslot Timeslot { get; set; } = null!;
}
