namespace Repo.Core.Models.user;

public class UpdateGroupDTO
{
    public int ID { get; set; }
    
    public string Name { get; set; } = null!;
    
    public int Admin_ID { get; set; }
}