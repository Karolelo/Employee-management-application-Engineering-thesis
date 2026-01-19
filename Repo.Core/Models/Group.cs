using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Group
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public int Admin_ID { get; set; }

    public int Deleted { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual Group_image? Group_image { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
