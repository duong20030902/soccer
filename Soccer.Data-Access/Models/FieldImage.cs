using System;
using System.Collections.Generic;

namespace Soccer.Data_Access.Models;

public partial class FieldImage
{
    public int ImageId { get; set; }

    public int FieldId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Field Field { get; set; } = null!;
}
