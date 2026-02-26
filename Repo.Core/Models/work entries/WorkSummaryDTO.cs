namespace Repo.Server.WorkTimeModule.DTOs;

public class WorkSummaryDTO
{
    public int User_ID { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }

    public decimal TotalHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalAmount { get; set; }
}