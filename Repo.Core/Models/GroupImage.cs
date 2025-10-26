using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class GroupImage
{
    public int GROUP_ID { get; set; }

    public string? Path { get; set; }

    public virtual Group GROUP { get; set; } = null!;
}
