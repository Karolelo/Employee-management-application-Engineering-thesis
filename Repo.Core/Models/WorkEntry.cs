using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class WorkEntry
{
    public int ID { get; set; }

    public int WorkTable_ID { get; set; }

    public int? Task_ID { get; set; }

    public DateTime Work_Date { get; set; }

    public decimal Hours_Worked { get; set; }

    public string? Comment { get; set; }

    public virtual Task? Task { get; set; }

    public virtual WorkTable WorkTable { get; set; } = null!;
}
