using System;

public class User
{
    public int Id { get; set;}
    public string Email { get; set;} = "";
    public string DisplayName { get; set; } = "";
    public string PasswordHash { get; set; } = ""; //he plaintext password is never stored, ever, not even temporarily on this model
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiery { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}