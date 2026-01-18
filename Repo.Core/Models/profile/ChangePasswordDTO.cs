namespace Repo.Core.Models.profile;

public class ChangePasswordDTO
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}