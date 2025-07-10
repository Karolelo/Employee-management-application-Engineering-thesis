using System;
using System.Collections.Generic;

namespace Repo.Core.Models;

public partial class Document
{
    public int ID { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Document_Path { get; set; } = null!;

    public int DocumentType_ID { get; set; }

    public int User_ID { get; set; }

    public virtual DocumentType DocumentType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
