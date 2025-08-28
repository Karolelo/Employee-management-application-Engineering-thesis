using System.ComponentModel.DataAnnotations;

namespace Repo.Server.GradeModule.DTOs;

public class GradeMiniDTO
{
    [Required]
    [Range(0, 100)]
    public decimal Grade { get; set; }
    
    [Required]
    public string Description { get; set; } = null!;
    
    [Required]
    public DateOnly Start_Date { get; set; }
    
    [Required]
    public DateOnly Finish_Date { get; set; }
}