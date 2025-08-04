using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Role
{
    public int ID { get; set; }

    public string Role_Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
