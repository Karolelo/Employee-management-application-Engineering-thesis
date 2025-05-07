using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Contribution
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public decimal Value { get; set; }

    public virtual ICollection<HireHelper> HireHelpers { get; set; } = new List<HireHelper>();
}
