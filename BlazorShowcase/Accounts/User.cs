using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorShowcase.Accounts;

[Table(nameof(User))]
public class User
{
    [Key] public Guid Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public virtual IList<Session> Sessions { get; set; }
}

public enum UserRole
{
    Admin,
    Normal,
}