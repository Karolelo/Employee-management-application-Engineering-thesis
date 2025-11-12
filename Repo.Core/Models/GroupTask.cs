using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class GroupTask
{
    public int Group_ID { get; set; }

    public int Task_ID { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
