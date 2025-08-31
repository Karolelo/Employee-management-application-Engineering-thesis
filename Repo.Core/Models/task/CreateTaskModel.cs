using System.ComponentModel.DataAnnotations;
using Repo.Core.MyAttributes;

namespace Repo.Core.Models.task;

public class CreateTaskModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100,ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = null!;
    [FutureDate]
    public DateTime Start_Time { get; set; }
    [Range(typeof(TimeSpan), "00:1:00", "365:00:00")]
    public TimeSpan Estimated_Time { get; set; }
    
    public string Priority { get; set; }

    public string Status { get; set; }
}