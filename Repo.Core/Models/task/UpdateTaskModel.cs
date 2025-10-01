namespace Repo.Core.Models.task;

public class UpdateTaskModel
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public DateTime Start_Time { get; set; }

    public int Estimated_Time { get; set; }
    
    public string Priority { get; set; }

    public string Status { get; set; }
}