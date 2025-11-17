namespace Repo.Core.Models.user;

public class CreateGroupDTO
{
    public string Name { get; set; } = null!;
    
    public string Description { get; set; }

    public int Admin_ID { get; set; }
}