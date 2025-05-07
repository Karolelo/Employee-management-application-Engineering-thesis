using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class BenefitType
{
    public int ID { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Benefit> Benefits { get; set; } = new List<Benefit>();
}
