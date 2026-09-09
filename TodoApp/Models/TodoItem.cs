//data model (the row in your table)
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

public partial class TodoItem : ObservableObject { //auto-increment primary key


    public int Id {get; set;} 
    public string Title {get; set;} = "";
    public DateTime DueDate {get; set;}
    public string Priority {get; set;} = "Low";
    public string Tag {get; set;} = "Others";
    public DateTime CreatedAt {get; set;} = DateTime.Now; //for "sort by date added"

    [ObservableProperty] private bool _isDone;

}

