using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Type
{
    public int ID { get; set; }

    public string Type1 { get; set; } = null!;

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
