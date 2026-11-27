using System;
using System.Collections;
using System.Collections.Generic;

public class User
{
    public int Id { get; set;}
    public string Email { get; set;} = "";
    public ICollection<TodoItem> Tasks { get; set; } = new List<TodoItem>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public string DisplayName { get; set; } = "";
    public string PasswordHash { get; set; } = ""; //he plaintext password is never stored, ever, not even temporarily on this model
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<TodoList> OwnedLists { get; set; } = new List<TodoList>();
    public ICollection<ListMembership> Memberships { get; set; } = new List<ListMembership>();
    
}