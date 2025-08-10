namespace Repo.Core.Models;

public partial class RefreshToken
{
    public int ID { get; set; }

    public int User_ID { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpireDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public virtual User User { get; set; } = null!;
}