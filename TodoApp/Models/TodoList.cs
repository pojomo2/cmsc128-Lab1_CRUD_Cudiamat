using System;
using System.Collections.Generic;

public class TodoList
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<ListMembership> Memberships { get; set; } = new List<ListMembership>();
    public ICollection<TodoItem> Tasks { get; set; } = new List<TodoItem>();
}

public class ListMembership
{

    public int ListId { get; set; }
    public TodoList List { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.Now;

}




