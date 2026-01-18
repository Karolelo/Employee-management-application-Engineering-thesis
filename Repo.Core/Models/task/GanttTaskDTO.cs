namespace Repo.Core.Models.DTOs;

public class GanttTaskDTO
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public DateTime Start_Time { get; set; }
    public DateTime End_Time { get; set; }          // Start_Time + Estimated_Time
    public string Priority { get; set; } = null!;
    public string Status { get; set; } = null!;
    
    public int OwnerUserId { get; set; }
    
    public ICollection<int> Dependencies { get; set; } = new List<int>();
}