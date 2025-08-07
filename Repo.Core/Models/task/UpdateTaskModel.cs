namespace Repo.Core.Models.task;

public class UpdateTaskModel
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public DateTime Start_Time { get; set; }

    public TimeSpan Estimated_Time { get; set; }
    //TODO pomyśleć czy nie łątwiej będzie z dawaniem id
    public string Priority { get; set; }

    public string Status { get; set; }
}