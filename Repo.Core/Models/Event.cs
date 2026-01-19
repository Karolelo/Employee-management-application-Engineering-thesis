using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Event
{
    public int ID { get; set; }

    public string Text { get; set; } = null!;

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string BackColor { get; set; } = null!;

    public int? Participants { get; set; }

    public int? Task_ID { get; set; }

    public int? AbsenceDay_ID { get; set; }

    public int? Course_ID { get; set; }

    public virtual AbsenceDay? AbsenceDay { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Task? Task { get; set; }
}
