using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class RelatedTask
{
    public int Main_Task_ID { get; set; }

    public int Related_Task_ID { get; set; }

    public int? Mandatory { get; set; }

    public virtual Task Main_Task { get; set; } = null!;

    public virtual Task Related_Task { get; set; } = null!;
}
