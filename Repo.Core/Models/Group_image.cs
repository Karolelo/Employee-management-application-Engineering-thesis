using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Group_image
{
    public int Group_ID { get; set; }

    public string Path { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
