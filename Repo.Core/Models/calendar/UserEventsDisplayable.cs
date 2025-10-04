namespace Repo.Core.Models.calendar;

public class UserEventsDisplayable
{
    public int ID { get; set; }
    public string Text { get; set; } = null!;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string BackColor { get; set; } = null!;
    public int? Task_ID { get; set; }
    public int? AbsenceDay_ID { get; set; }
    public int? Course_ID { get; set; }
}