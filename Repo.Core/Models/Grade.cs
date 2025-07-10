using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Grade
{
    public int ID { get; set; }

    public decimal Grade1 { get; set; }

    public string Description { get; set; } = null!;

    public DateOnly Start_Date { get; set; }

    public DateOnly Finish_Date { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
