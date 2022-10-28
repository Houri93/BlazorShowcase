using System.ComponentModel.DataAnnotations;

namespace BlazorShowcase.Accounts;

public class Session
{
    [Key] public Guid Id { get; set; }
    public Guid Token { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public string Ip { get; set; }
    public string Agent { get; set; }
    public bool Ened { get; set; }
}
