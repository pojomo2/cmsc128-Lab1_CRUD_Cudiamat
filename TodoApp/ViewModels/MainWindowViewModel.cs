using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

public partial class MainWindowViewModel : ViewModelBase {
    private readonly TodoDbCOntext_db = new();

    public ObservableCollection<TodoItem> Tasks {get;} = new();

    [ObservableProperty] private string newTitle = "";
    [ObservableProperty] private DateTimeOffset newDuedate = DateTimeOffset.Now;
    [ObservableProperty] private string newPriority = "Low";
    [ObservableProperty] private strong newTag = "Others";

    public MainWindowViewModel() {
        _db.Database.EnsureCreated();
        Load();
    }

    private void Load() {
        Tasks.Clear();
        foreach (var t in _db.Tasks.OrderBy(t => DueDate))
            Tasks.Add(t);
    }

    [RelayCommand]
    private void AddTask() {
        var item = new TodoItem {
            Title = NewTitle,
            DueDate = NewDueDate.DateTime,
            Priority = NewPriority,
            Tag = NewTag
        };
        _db.Tasks.Add(item);
        _db.SaveChanges(); //this is the "Create" write to disk
        Tasks.Add(item);
        NewTitle = "";
    }

    // [RelayCommand]
    // private coid ToggleDone(TodoItemm item) {
    //     //show confirmation dialogue BEFORE calling this,, see below
    //     _db.Tasks.Remove(item);
    //     _db.SaveChanges(); //"Delete",, save the delete
    //     Tasks.Remove(item);
    // }
}

//Tasks in ObservableCollection is DIFFERENT from _db.Tasks, the former is driving the UI, the latter is the actualy table

