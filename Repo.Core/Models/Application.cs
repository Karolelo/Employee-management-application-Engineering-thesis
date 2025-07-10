using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Application
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Template { get; set; } = null!;

    public int DocumentType_ID { get; set; }

    public virtual DocumentType DocumentType { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
