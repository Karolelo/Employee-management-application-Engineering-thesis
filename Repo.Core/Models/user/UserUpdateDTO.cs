using System.ComponentModel.DataAnnotations;
using Repo.Core.MyAttributes;

namespace Repo.Core.Models.user;

public class UserUpdateDTO
{   
    [Required(ErrorMessage = "The ID field is required.")]
    public int ID { get; set; }
    [Required(ErrorMessage = "Nickname is required")]
    public string Nickname { get; set; } = null!;
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Surname is required")]
    public string Surname { get; set; } = null!;
    
    [Login]
    public string Login { get; set; } = null!;
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
    
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;  
    [Role]
    public ICollection<string>? Roles { get; set; }
}