using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Group
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public int Admin_ID { get; set; }

    public int Deleted { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual GroupImage? GroupImage { get; set; }

    public virtual ICollection<GroupTask> GroupTasks { get; set; } = new List<GroupTask>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
