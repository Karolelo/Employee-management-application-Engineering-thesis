using Microsoft.EntityFrameworkCore.Metadata;

namespace Repo.Core.Models.DTOs;

public class TaskDTO
{
    //To be honest mrozinski tak chyba by było wygodniej 
    /*public TaskDTO(Task task)
    {
        this.ID = task.ID;
        this.Name = task.Name;
        this.Description = task.Description;
        this.Start_Time = task.Start_Time;
        this.Estimated_Time = task.Estimated_Time;
        this.Priority = task.Priority.Priority1;
        this.Status = task.Status.Status1;
    }*/
    public int ID { get; set; }
    
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public DateTime Start_Time { get; set; }

    public int Estimated_Time { get; set; }
    
    public string Priority { get; set; }

    public string Status { get; set; }
}

public class TaskWithRelatedDTO
{
    public TaskDTO Task { get; set; }
    public IReadOnlyList<TaskDTO> RelatedTasks { get; set; }
}