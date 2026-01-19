using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Task
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Start_Time { get; set; }

    public int Estimated_Time { get; set; }

    public int Creator_ID { get; set; }

    public int Deleted { get; set; }

    public int Priority_ID { get; set; }

    public int Status_ID { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual Priority Priority { get; set; } = null!;

    public virtual ICollection<RelatedTask> RelatedTaskMain_Tasks { get; set; } = new List<RelatedTask>();

    public virtual ICollection<RelatedTask> RelatedTaskRelated_Tasks { get; set; } = new List<RelatedTask>();

    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<WorkEntry> WorkEntries { get; set; } = new List<WorkEntry>();

    public virtual ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
