using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Status
{
    public int ID { get; set; }

    public string Status1 { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
