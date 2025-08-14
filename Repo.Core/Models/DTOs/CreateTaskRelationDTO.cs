namespace Repo.Core.Models.DTOs;

public class CreateTaskRelationDTO
{
    public int RelatedTaskID { get; set; }
}

public record TaskRelationDTO(int MainTaskID, int RelatedTaskID);