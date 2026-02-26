namespace Repo.Server.WorkTimeModule.DTOs;

public class WorkEntryDTO
{
    public int ID { get; set; }
    public int WorkTable_ID { get; set; }
    public int? Task_ID { get; set; }
    public DateOnly Work_Date { get; set; }
    public decimal Hours_Worked { get; set; }
    public string? Comment { get; set; }

    public string? TaskName { get; set; }
    public string? UserNickname { get; set; }
    public int? UserID { get; set; }
}
