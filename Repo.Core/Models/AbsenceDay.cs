using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class AbsenceDay
{
    public int ID { get; set; }

    public int WorkTable_ID { get; set; }

    public DateOnly Start_Date { get; set; }

    public DateOnly Finish_Date { get; set; }

    public virtual WorkTable WorkTable { get; set; } = null!;
}
