using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Target
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Start_Time { get; set; }

    public DateTime? Finish_Time { get; set; }

    public int? Tag_ID { get; set; }

    public virtual Tag? Tag { get; set; }
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
