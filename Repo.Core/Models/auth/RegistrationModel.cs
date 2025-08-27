using System.ComponentModel.DataAnnotations;
using Repo.Core.MyAttributes;

namespace Repo.Core.Models.auth;

public class RegistrationModel
{
    [Required(ErrorMessage = "Nickname is required")]
    public string? Nickname { get; set; }
    
    [Login]
    public string Login { get; set; }
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    
    public ICollection<string>? Role { get; set; }
}