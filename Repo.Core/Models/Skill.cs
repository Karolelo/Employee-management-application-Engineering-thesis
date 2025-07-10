using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Skill
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Type_ID { get; set; }

    public virtual Type Type { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
