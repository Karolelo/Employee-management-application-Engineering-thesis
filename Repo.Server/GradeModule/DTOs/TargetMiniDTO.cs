using System.ComponentModel.DataAnnotations;

namespace Repo.Server.GradeModule.DTOs;

public class TargetMiniDTO
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [Required]
    public string Description { get; set; } = null!;
    
    [Required]
    public DateTime Start_Time { get; set; }
    
    public DateTime? Finish_Time { get; set; }
    
    public string? Tag { get; set; }
}