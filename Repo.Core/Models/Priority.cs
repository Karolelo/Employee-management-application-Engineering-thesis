using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Priority
{
    public int ID { get; set; }

    public string Priority1 { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
