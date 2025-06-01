using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class Field
{
    public int FieldId { get; set; }

    public string FieldName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public decimal PricePerHour { get; set; }

    public virtual ICollection<FieldImage> FieldImages { get; set; } = new List<FieldImage>();

    public virtual ICollection<FieldSchedule> FieldSchedules { get; set; } = new List<FieldSchedule>();
}
