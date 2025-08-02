using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class User
{
    public int ID { get; set; }

    public string Nickname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte AdminPrivileges { get; set; }

    public int Deleted { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<HireHelper> HireHelpers { get; set; } = new List<HireHelper>();

    public virtual ICollection<Target> Targets { get; set; } = new List<Target>();

    public virtual ICollection<WorkTable> WorkTables { get; set; } = new List<WorkTable>();

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Benefit> Benefits { get; set; } = new List<Benefit>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
