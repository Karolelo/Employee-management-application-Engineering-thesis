using System.ComponentModel.DataAnnotations;

namespace Repo.Server.GradeModule.DTOs;

public class CourseMiniDTO
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [Required]
    public string Description { get; set; } = null!;
    
    [Required]
    public DateOnly Start_Date { get; set; }
    
    [Required]
    public DateOnly Finish_Date { get; set; }
}