//data model (the row in your table)
using System;

public class TodoItem { //auto-increment primary key
    public int Id {get; set;} 
    public string Title {get; set;} = "";
    public DateTime DueDate {get; set;}
    public string Priority {get; set;} = "Low";
    public string Tag {get; set;} = "Others";
    public bool IsDone {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now; //for "sort by date added"
}