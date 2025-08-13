namespace Repo.Core.Models.DTOs;

public class TaskDTO
{
    public int ID { get; set; }
    
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public DateTime Start_Time { get; set; }

    public TimeSpan Estimated_Time { get; set; }
    
    public string Priority { get; set; }

    public string Status { get; set; }
}

public class TaskWithRelatedDTO
{
    public TaskDTO Task { get; set; }
    public IReadOnlyList<TaskDTO> RelatedTasks { get; set; }
}