using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class HireHelper
{
    public int ID { get; set; }

    public decimal Hourly_Rate { get; set; }

    public decimal Netto_Salary { get; set; }

    public int User_ID { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
}
