using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TodoApp.ViewModels;

namespace TodoApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private readonly TodoDbContext _db = new();

    public ObservableCollection<TodoItem> Tasks {get;} = new();

    [ObservableProperty] private string newTitle = "";
    [ObservableProperty] private DateTimeOffset newDueDate = DateTimeOffset.Now;
    [ObservableProperty] private string newPriority = "Low";
    [ObservableProperty] private string newTag = "Others";

    public MainWindowViewModel() {
        _db.Database.EnsureCreated();
        Load();
    }

    private void Load() {
        Tasks.Clear();
        foreach (var t in _db.Tasks.OrderBy(t => t.DueDate))
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

    [RelayCommand]
    private void ToggleDone(TodoItem item) {
        item.IsDone = !item.IsDone;
        _db.SaveChanges(); //"Update"
    }

    [RelayCommand]
    private void DeleteTask(TodoItem item) {
        //show confirmation dialogue BEFORE calling this,, see below
        _db.Tasks.Remove(item);
        _db.SaveChanges(); //"Delete",, save the delete
        Tasks.Remove(item);
    }

    private string _newProperty = string.Empty;
    public string NewProperty
    {
        get => _newProperty;
        set => SetProperty(ref _newProperty, value);
    }
}

//Tasks in ObservableCollection is DIFFERENT from _db.Tasks, the former is driving the UI, the latter is the actualy table

