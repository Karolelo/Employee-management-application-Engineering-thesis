using System.ComponentModel.DataAnnotations;

namespace Repo.Core.Models.user.annoucement;

public class AnnoucementDTO
{
    public int ID { get; set; }
    
    [Required]
    public int Group_ID { get; set; }
    
    [Required]
    public string Title {get; set;}
    [Required]
    public string Text {get; set;}
    
}