using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class WorkTask
{
    public int WorkTable_ID { get; set; }

    public int Task_ID { get; set; }

    public int Finished { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual WorkTable WorkTable { get; set; } = null!;
}