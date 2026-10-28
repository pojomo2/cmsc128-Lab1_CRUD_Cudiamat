//data model (the row in your table)
using System;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class TodoItem : ObservableObject { 


    public int Id {get; set;} //auto-increment primary key
    public int UserId { get; set; }
    public User User {get; set;} = null!;
    public string Title {get; set;} = "";
    public DateTime DueDate {get; set;}
    public string Priority {get; set;} = "Low";
    public string Tag {get; set;} = "Personal";
    public DateTime CreatedAt {get; set;} = DateTime.Now; //for "sort by date added"
    public int? ListId { get; set; }
    public TodoList? List { get; set; }

    [ObservableProperty] private bool _isDone;




}

