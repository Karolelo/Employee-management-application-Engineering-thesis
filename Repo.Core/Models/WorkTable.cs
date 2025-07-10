using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class WorkTable
{
    public int ID { get; set; }

    public decimal Hourly_Rate { get; set; }

    public string Account_Number { get; set; } = null!;

    public int User_ID { get; set; }

    public virtual ICollection<AbsenceDay> AbsenceDays { get; set; } = new List<AbsenceDay>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();
}
