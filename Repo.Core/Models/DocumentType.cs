using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class DocumentType
{
    public int ID { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
