using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class Timeslot
{
    public int TimeslotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<FieldSchedule> FieldSchedules { get; set; } = new List<FieldSchedule>();
}
