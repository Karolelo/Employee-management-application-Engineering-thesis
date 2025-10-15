namespace Repo.Core.Models.user;

public class UserDto
{
    public UserDto(User user)
    {
        ID = user.ID;
        Email = user.Email;
        Name = user.Name;
        Surname = user.Surname;
        Nickname = user.Nickname;
        Login = user.Login;
        Password = user.Password;
        Roles = user.Roles.Select(r => r.Role_Name).ToList();
    }
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<string> Roles { get; set; }
    
}