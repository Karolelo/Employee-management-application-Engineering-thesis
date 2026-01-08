namespace Repo.Server.GradeModule.DTOs;

public class CourseDTO
{
    public int ID { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public DateOnly Start_Date { get; set; }
    
    public DateOnly Finish_Date { get; set; }
}