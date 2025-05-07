using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Benefit
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Cost { get; set; }

    public int BenefitType_ID { get; set; }

    public virtual BenefitType BenefitType { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
