using Repo.Core.Models.DTOs;

namespace Repo.Core.Models.user;

public class GroupDTO
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public int Admin_ID { get; set; }
    
    public string? Description {get; set;}
}