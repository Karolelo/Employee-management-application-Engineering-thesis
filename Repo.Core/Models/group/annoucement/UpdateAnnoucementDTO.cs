using System.ComponentModel.DataAnnotations;

namespace Repo.Core.Models.user.annoucement;

public class UpdateAnnoucementDTO
{
    
    public int ID { get; set; }
    
    [Required]
    public string Title {get; set;}
    
    [Required]
    public string Text {get; set;}
}