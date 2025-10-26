using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Announcement
{
    public int ID { get; set; }

    public string? Title { get; set; }

    public string? Text { get; set; }

    public int? Group_ID { get; set; }

    public virtual Group? Group { get; set; }
}
