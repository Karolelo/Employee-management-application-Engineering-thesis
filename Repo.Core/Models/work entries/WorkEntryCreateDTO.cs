using System.ComponentModel.DataAnnotations;

namespace Repo.Server.WorkTimeModule.DTOs;

public class WorkEntryCreateDTO
{
    [Required]
    public DateOnly Work_Date { get; set; }

    [Required]
    [Range(0.25, 24)]
    public decimal Hours_Worked { get; set; }

    public int? Task_ID { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}