namespace Repo.Server.GradeModule.DTOs;

public class TargetDTO
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;

    public DateTime Start_Time { get; set; }
    
    public DateTime? Finish_Time { get; set; }
    
    public string? Tag { get; set; }
}