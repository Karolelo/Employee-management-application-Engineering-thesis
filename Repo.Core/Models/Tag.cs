using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Tag
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Target> Targets { get; set; } = new List<Target>();
}
