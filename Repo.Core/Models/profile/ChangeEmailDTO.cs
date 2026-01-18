namespace Repo.Core.Models.profile;

public class ChangeEmailDTO
{
    public string CurrentPassword { get; set; } = null!;
    public string NewEmail { get; set; } = null!;
}