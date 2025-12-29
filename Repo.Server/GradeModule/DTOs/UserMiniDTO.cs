namespace Repo.Server.GradeModule.DTOs;

public class UserMiniDTO
{
    public int ID { get; set; }
    
    public string Login { get; set; } = null!;
    
    public string Nickname { get; set; } = null!;
}